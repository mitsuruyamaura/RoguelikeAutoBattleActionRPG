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
    /// �h���b�v����A�C�e���̃v���t�@�u���擾
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public DropBoxBase GetDropItemPrefab(ItemType itemType) {
        return itemType switch {
            ItemType.Goal => goalPointPrefab,
            ItemType.Coin => coinPrefab,
            ItemType.Food => foodPrefab,
            _ => null
        };
    }

    /// <summary>
    /// �w�肳�ꂽ�A�C�e���̐���
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="generatePos"></param>
    public void GenerateDropItem(ItemType itemType, Vector2 generatePos) {
        DropBoxBase dropItem = Instantiate(GetDropItemPrefab(itemType), generatePos, Quaternion.identity);

    }
}
