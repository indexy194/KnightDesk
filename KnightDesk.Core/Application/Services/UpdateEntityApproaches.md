# Entity Update Approaches với AutoMapper

## Vấn đề với entities có nhiều trường (1000+ properties)

Khi entity có rất nhiều trường, việc update từng property sẽ rất cồng kềnh:

```csharp
// ❌ Không practical với entities lớn
existingEntity.Field1 = dto.Field1;
existingEntity.Field2 = dto.Field2;
// ... 998 fields more
existingEntity.Field1000 = dto.Field1000;
```

## ✅ Giải pháp: AutoMapper với các approaches khác nhau

### **Approach 1: Map(source, destination) - RECOMMENDED**
```csharp
// ✅ Best approach cho entities lớn
var existingEntity = await GetByIdAsync(id); // EF tracks this
_mapper.Map(dto, existingEntity); // Update all properties, keep tracking
await UpdateAsync(existingEntity); // EF generates UPDATE WHERE Id = id
```

**Ưu điểm:**
- Giữ nguyên EF tracking
- Preserve Id và audit fields
- Chỉ 1 dòng code cho hàng ngàn fields
- Performance tốt

### **Approach 2: Detach → Map → Attach**
```csharp
// Alternative approach
var existingEntity = await GetByIdAsync(id);
_context.Entry(existingEntity).State = EntityState.Detached; // Detach
var updatedEntity = _mapper.Map<Entity>(dto); // Map to new entity
updatedEntity.Id = id; // Set correct Id
_context.Entry(updatedEntity).State = EntityState.Modified; // Attach as modified
await SaveChangesAsync();
```

### **Approach 3: ExecuteUpdateAsync (EF Core 7+)**
```csharp
// Modern EF Core approach (if available)
await _context.Accounts
    .Where(a => a.Id == id)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(a => a.Username, dto.Username)
        .SetProperty(a => a.Password, dto.Password)
        // ... other properties
    );
```

## AutoMapper Configuration cho Update

```csharp
CreateMap<UpdateAccountDTO, Account>()
    .ForMember(dest => dest.Id, opt => opt.Ignore()) // ✅ Preserve Id
    .ForMember(dest => dest.ServerInfo, opt => opt.Ignore()) // ✅ Ignore navigation
    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // ✅ Preserve audit
    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
```

## Kết luận

**Approach 1** (`_mapper.Map(dto, existingEntity)`) là tốt nhất vì:
- Simple và clean
- Tự động handle hàng ngàn fields
- Preserve EF tracking
- Ignore được các fields không muốn update
- Performance tốt nhất
