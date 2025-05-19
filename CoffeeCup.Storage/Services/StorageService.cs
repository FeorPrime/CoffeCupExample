using CoffeeCup.Contracts;
using CoffeeCup.Contracts.Resources;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCup.Storage.Services;

public class StorageService
{
    private readonly CoffeeCupStorageContext _context;
    public StorageService(CoffeeCupStorageContext context) { _context = context; }

    public async Task<ResourcesDto> CheckResourcesAsync(CancellationToken cancellation = default) => (await _context.Resources.FirstOrDefaultAsync(cancellation)).ToResources();
    
    public async Task<bool> TakeResourcesAsync(ResourcesDto payload, CancellationToken cancellation = default)
    {
        var storage = _context.Resources.FirstOrDefault();
        if (storage is null) return false;
        
        storage.Coffee -= payload.Coffee ?? 0f;
        storage.Water -= payload.Water ?? 0f;

        if (!(storage.Coffee > 0) || !(storage.Water > 0)) return false;
        
        await _context.SaveChangesAsync(cancellation);
        return true;
    }

    public async Task StoreResourcesAsync(ResourcesDto payload, CancellationToken cancellation = default)
    {
        var storage = _context.Resources.FirstOrDefault();
        
        if (storage == null)
        {
            _context.Resources.Add(new ResourceEntity
            {
                Coffee = payload.Coffee ?? 0f,
                Water = payload.Water ?? 0f,
            });
            await _context.SaveChangesAsync(cancellation);
            return;
        };
        
        storage.Coffee += payload.Coffee ?? 0;    
        storage.Water += payload.Water ?? 0;
            
        await _context.SaveChangesAsync(cancellation);
    }
    
    public async Task PruneResourcesAsync(CancellationToken cancellation = default)
    {
        var storage = _context.Resources.FirstOrDefault();
        if (storage is not null)
        {
            storage.Coffee = 0f;
            storage.Water = 0f;
            
            await _context.SaveChangesAsync(cancellation);
        }
    }
}