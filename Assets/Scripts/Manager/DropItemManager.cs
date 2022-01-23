using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    public static DropItemManager instance;

    [SerializeField]
    private DropBoxBase goalPointPrefab;

    [SerializeField]
    private DropBoxBase coinPrefab;
    
    [SerializeField]
    private DropBoxBase foodPrefab;

    [SerializeField]
    private DropBoxBase treasureWeaponPrefab;


    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ドロップするアイテムのプレファブを取得
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public DropBoxBase GetDropItemPrefab(ItemType itemType) {
        return itemType switch {
            ItemType.Goal => goalPointPrefab,
            ItemType.Coin => coinPrefab,
            ItemType.Food => foodPrefab,
            ItemType.Weapon => treasureWeaponPrefab,
            _ => null
        };
    }

    /// <summary>
    /// 指定されたアイテムの生成
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="generatePos"></param>
    public DropBoxBase GenerateDropItem(ItemType itemType, Vector2 generatePos) {
        return Instantiate(GetDropItemPrefab(itemType), generatePos, Quaternion.identity);
    }
}
