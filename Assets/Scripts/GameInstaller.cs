using Application.Services;
using Application.UseCases;
using Config.ScriptableObjects;
using Infrastructure.Factories;
using Infrastructure.Save;
using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    [SerializeField] private InventoryConfigSO inventoryConfig;
    [SerializeField] private ItemRegistrySO itemRegistry;

    public static GameInstaller Instance { get; private set; }

    public InventoryService InventoryService { get; private set; }
    public CoinService CoinService { get; private set; }
    public ItemFactory ItemFactory { get; private set; }
    public ShootService ShootService { get; private set; }
    public InventoryUseCases UseCases { get; private set; }
    public ItemRegistrySO ItemRegistry => itemRegistry;
    public InventoryConfigSO InventoryConfig => inventoryConfig;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        var saveService = new EasySaveService(inventoryConfig);
        InventoryService = new InventoryService(saveService, itemRegistry);
        CoinService = new CoinService(InventoryService);
        ItemFactory = new ItemFactory(itemRegistry);
        ShootService = new ShootService(InventoryService, itemRegistry);
        UseCases = new InventoryUseCases(
            InventoryService,
            CoinService,
            ItemFactory,
            ShootService,
            itemRegistry,
            inventoryConfig
        );
    }
}
