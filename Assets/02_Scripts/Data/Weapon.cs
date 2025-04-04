using UnityEngine;
using UnityEngine.U2D;

public class Weapon : MonoBehaviour
{
    public int getId;

    public WeaponData weaponData;
    public SpriteRenderer _spriteRenderer;
    SpriteAtlas _atlasSprite;

    public void Start()
    {
        Init();
    }

    void Init()
    {
        getId = Random.Range(1, DataManager.Instance.weaponCount);
        weaponData = DataManager.Instance.GetWeaponById(getId);
        _atlasSprite = Resources.Load<SpriteAtlas>("WeaponSprite/Weapon");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _atlasSprite.GetSprite($"{getId}");
    }
}
