using KnightDesk.Presentation.WPF.Constants;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KnightDesk.Presentation.WPF.Services
{
    public interface ITcpServices
    {
        void StartTcpServer(int port, Action<bool> callback = null);
        void StopTcpServer(Action<bool> callback = null);
        void SendCommandToGameAsync(int accountId, string command, Action<bool> callback = null);
        bool SendCommandToGame(int accountId, string command, Action<string> responseCallback = null);
        bool SendCommandToGameNoResponse(int accountId, string command);
        bool IsClientConnected(int accountId);
        bool IsServerRunning { get; }
        
        // Events for VM to handle
        event Action<int, string> CommandReceived;
        event Action<int> ClientConnected;
        event Action<int> ClientDisconnected;

        void BroadcastToAllClients(string command);
    }
    public class TcpServices : ITcpServices
    {
        private TcpListener _tcpListener;
        private readonly Dictionary<int, TcpClient> _connectedClients = new Dictionary<int, TcpClient>();
        private readonly Dictionary<int, NetworkStream> _clientStreams = new Dictionary<int, NetworkStream>();
        private readonly object _lockObject = new object();
        private bool _isServerRunning = false;

        public event Action<int, string> CommandReceived;
        public event Action<int> ClientConnected;
        public event Action<int> ClientDisconnected;

        //new
        public void StartTcpServer(int port, Action<bool> callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = false;
                try
                {
                    // Stop existing server if running
                    if (_isServerRunning)
                    {
                        StopTcpServerInternal();
                    }

                    // Try to start on the requested port
                    if (TryStartOnPort(port))
                    {
                        _isServerRunning = true;
                        Console.WriteLine($"TCP Server started on port {port}");
                        
                        // Start accepting connections
                        ThreadPool.QueueUserWorkItem(__ => AcceptConnections());
                        result = true;
                    }
                    else
                    {
                        // Try alternative ports if default port is busy
                        int[] alternativePorts = { port + 1, port + 2, port + 3, port - 1, port - 2 };
                        foreach (int altPort in alternativePorts)
                        {
                            if (altPort > 1024 && altPort < 65536 && TryStartOnPort(altPort))
                            {
                                _isServerRunning = true;
                                Console.WriteLine($"TCP Server started on alternative port {altPort} (original port {port} was busy)");
                                
                                // Start accepting connections
                                ThreadPool.QueueUserWorkItem(__ => AcceptConnections());
                                result = true;
                                break;
                            }
                        }
                        
                        if (!result)
                        {
                            Console.WriteLine($"Failed to start TCP server on port {port} or any alternative ports");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start TCP server: {ex.Message}");
                    result = false;
                }

                if (callback != null) callback(result);
            });
        }

        private bool TryStartOnPort(int port)
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, port);
                _tcpListener.Start();
                return true;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                Console.WriteLine($"Port {port} is already in use");
                _tcpListener?.Stop();
                _tcpListener = null;
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting TCP listener on port {port}: {ex.Message}");
                _tcpListener?.Stop();
                _tcpListener = null;
                return false;
            }
        }

        private void StopTcpServerInternal()
        {
            try
            {
                _isServerRunning = false;
                
                // Send shutdown command to all connected clients first
                lock (_lockObject)
                {
                    var clientsToNotify = new List<int>(_connectedClients.Keys);
                    foreach (var accountId in clientsToNotify)
                    {
                        try
                        {
                            if (_connectedClients.ContainsKey(accountId) && _connectedClients[accountId].Connected)
                            {
                                var stream = _clientStreams[accountId];
                                var shutdownBytes = Encoding.UTF8.GetBytes(GameCommandConstants.CLIENT_SHUTDOWN_COMMAND);
                                stream.Write(shutdownBytes, 0, shutdownBytes.Length);
                                stream.Flush();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending shutdown to client {accountId}: {ex.Message}");
                        }
                    }
                }

                // Wait a moment for clients to process shutdown
                Thread.Sleep(500);
                
                // Close all client connections
                lock (_lockObject)
                {
                    foreach (var client in _connectedClients.Values)
                    {
                        try
                        {
                            client.Close();
                        }
                        catch { }
                    }
                    _connectedClients.Clear();
                    _clientStreams.Clear();
                }

                // Stop the listener
                _tcpListener?.Stop();
                _tcpListener = null;
                
                Console.WriteLine("TCP Server stopped internally");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping TCP server internally: {ex.Message}");
            }
        }

        public void StopTcpServer(Action<bool> callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = false;
                try
                {
                    StopTcpServerInternal();
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping TCP server: {ex.Message}");
                    result = false;
                }

                if (callback != null) callback(result);
            });
        }

        private void AcceptConnections()
        {
            while (_isServerRunning)
            {
                try
                {
                    var client = _tcpListener.AcceptTcpClient();
                    Console.WriteLine($"New client connected from {client.Client.RemoteEndPoint}");

                    // Handle each client in a separate thread
                    ThreadPool.QueueUserWorkItem(__ => HandleClient(client));
                }
                catch (Exception ex)
                {
                    if (_isServerRunning)
                    {
                        Console.WriteLine($"Error accepting connection: {ex.Message}");
                    }
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            int accountId = -1;
            try
            {
                var stream = client.GetStream();

                // Read the initial registration message
                var buffer = new byte[GameConfigConstants.DEFAULT_BUFFER_SIZE];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Expected format: "REGISTER|{accountId}"
                    if (message.StartsWith(GameCommandConstants.REGISTER_COMMAND + GameCommandConstants.COMMAND_SEPARATOR))
                    {
                        string[] parts = message.Split(GameCommandConstants.COMMAND_SEPARATOR[0]);
                        if (parts.Length >= 2 && int.TryParse(parts[1], out accountId))
                        {
                            if (RegisterGameClient(accountId, client))
                            {
                                // Send confirmation
                                var response = GameCommandConstants.REGISTERED_OK_RESPONSE;
                                var responseBytes = Encoding.UTF8.GetBytes(response);
                                stream.Write(responseBytes, 0, responseBytes.Length);
                                stream.Flush();

                                Console.WriteLine($"Client registered for account {accountId}");
                                ClientConnected?.Invoke(accountId);

                                // Continue listening for commands from this client
                                ListenForCommands(accountId, stream);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                if (accountId > 0)
                {
                    Console.WriteLine($"cac abcd: {accountId}");
                    UnregisterGameClient(accountId);
                    ClientDisconnected?.Invoke(accountId);
                }

                try
                {
                    client.Close();
                }
                catch { }
            }
        }

        private void ListenForCommands(int accountId, NetworkStream stream)
        {
            var buffer = new byte[GameConfigConstants.DEFAULT_BUFFER_SIZE];

            while (_isServerRunning && _connectedClients.ContainsKey(accountId))
            {
                try
                {
                    // Check if data is available before reading to avoid blocking
                    if (!stream.DataAvailable)
                    {
                        Thread.Sleep(50); // Small delay to prevent busy waiting
                        continue;
                    }

                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received from account {accountId}: {command}");

                        // Raise event for command processing
                        CommandReceived?.Invoke(accountId, command);
                    }
                    else
                    {
                        // Client disconnected
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading from client {accountId}: {ex.Message}");
                    break;
                }
            }
        }

        public bool RegisterGameClient(int accountId, TcpClient client)
        {
            lock (_lockObject)
            {
                if (_connectedClients.ContainsKey(accountId))
                {
                    // Close existing connection
                    try
                    {
                        _connectedClients[accountId].Close();
                        _clientStreams.Remove(accountId);
                    }
                    catch { }
                }

                _connectedClients[accountId] = client;
                _clientStreams[accountId] = client.GetStream();

                return true;
            }
        }

        public void UnregisterGameClient(int accountId)
        {
            lock (_lockObject)
            {
                if (_connectedClients.ContainsKey(accountId))
                {
                    try
                    {
                        _clientStreams[accountId].Close();
                    }
                    catch { }

                    _connectedClients.Remove(accountId);
                    _clientStreams.Remove(accountId);
                }
            }
        }

        public bool SendCommandToGame(int accountId, string command, Action<string> responseCallback = null)
        {
            lock (_lockObject)
            {
                if (!_connectedClients.ContainsKey(accountId))
                {
                    Console.WriteLine($"Cannot send command to account {accountId}: client not registered");
                    return false;
                }

                var client = _connectedClients[accountId];
                if (!client.Connected)
                {
                    Console.WriteLine($"Cannot send command to account {accountId}: client disconnected");
                    return false;
                }

                try
                {
                    Console.WriteLine($"Sending to account {accountId}: {command}");
                    var stream = _clientStreams[accountId];
                    var commandBytes = Encoding.UTF8.GetBytes(command);

                    stream.Write(commandBytes, 0, commandBytes.Length);
                    stream.Flush();

                    // If expecting a response, read it with timeout
                    if (responseCallback != null)
                    {
                        var buffer = new byte[GameConfigConstants.DEFAULT_BUFFER_SIZE];
                        
                        // Set read timeout to avoid indefinite blocking
                        stream.ReadTimeout = 5000; // 5 seconds timeout
                        
                        try
                        {
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);

                            if (bytesRead > 0)
                            {
                                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                responseCallback(response);
                            }
                            else
                            {
                                Console.WriteLine($"No response received from account {accountId}");
                            }
                        }
                        catch (System.IO.IOException timeoutEx)
                        {
                            Console.WriteLine($"Timeout waiting for response from account {accountId}: {timeoutEx.Message}");
                        }
                        finally
                        {
                            // Reset timeout
                            stream.ReadTimeout = System.Threading.Timeout.Infinite;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending command to account {accountId}: {ex.Message}");
                    
                    // Remove disconnected client
                    try
                    {
                        _connectedClients.Remove(accountId);
                        _clientStreams.Remove(accountId);
                    }
                    catch { }
                    
                    return false;
                }
            }
        }

        public bool SendCommandToGameNoResponse(int accountId, string command)
        {
            lock (_lockObject)
            {
                if (!_connectedClients.ContainsKey(accountId))
                {
                    Console.WriteLine($"Cannot send command to account {accountId}: client not registered");
                    return false;
                }

                var client = _connectedClients[accountId];
                if (!client.Connected)
                {
                    Console.WriteLine($"Cannot send command to account {accountId}: client disconnected");
                    return false;
                }

                try
                {
                    Console.WriteLine($"Sending (no response) to account {accountId}: {command}");
                    var stream = _clientStreams[accountId];
                    var commandBytes = Encoding.UTF8.GetBytes(command);

                    stream.Write(commandBytes, 0, commandBytes.Length);
                    stream.Flush();

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending command (no response) to account {accountId}: {ex.Message}");
                    
                    // Remove disconnected client
                    try
                    {
                        _connectedClients.Remove(accountId);
                        _clientStreams.Remove(accountId);
                    }
                    catch { }
                    
                    return false;
                }
            }
        }

        public void SendCommandToGameAsync(int accountId, string command, Action<bool> callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = SendCommandToGameNoResponse(accountId, command);
                if (callback != null) callback(result);
            });
        }

        public bool IsClientConnected(int accountId)
        {
            lock (_lockObject)
            {
                if (!_connectedClients.ContainsKey(accountId))
                {
                    return false;
                }

                var client = _connectedClients[accountId];
                if (!client.Connected)
                {
                    // Clean up disconnected client
                    try
                    {
                        _connectedClients.Remove(accountId);
                        _clientStreams.Remove(accountId);
                    }
                    catch { }
                    return false;
                }

                return true;
            }
        }

        public bool IsServerRunning => _isServerRunning;

        public void BroadcastToAllClients(string command)
        {
            lock (_lockObject)
            {
                foreach (var accountId in _connectedClients.Keys)
                {
                    SendCommandToGameAsync(accountId, command);
                }
            }
        }
    }
    
}
