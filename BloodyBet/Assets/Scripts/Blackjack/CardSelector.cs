<<<<<<< HEAD
using UnityEngine;

public class CardSelector : MonoBehaviour
{

    public Rank card;

    private const float UV_SCALE_X = 0.2f;
    private const float UV_SCALE_Y = 0.334f;

    private Renderer _rend;

    void OnValidate()
    {
        if (!TryGetRenderer())
            return;

        ApplyCard(_rend.sharedMaterial);
    }

    void Start()
    {
        TryGetRenderer();
        ApplyCard(_rend.material);
    }

    bool TryGetRenderer()
    {
        if (_rend != null)
            return true;

        _rend = GetComponent<Renderer>();
        return _rend != null;
    }

    public void SetCard(Rank newCard)
    {
        card = newCard;
        TryGetRenderer();
        ApplyCard(_rend.sharedMaterial);
    }

    void ApplyCard(Material mat)
    {
        if (mat == null) return;

        mat.SetVector("_UVScale", new Vector4(UV_SCALE_X, UV_SCALE_Y, 0, 0));

        Vector2Int grid = GetCardGridPosition(card);

        float offsetX = grid.x * UV_SCALE_X;
        float offsetY = grid.y * UV_SCALE_Y;

        mat.SetVector("_UVOffset", new Vector4(offsetX, offsetY, 0, 0));
    }

    Vector2Int GetCardGridPosition(Rank c)
    {
        switch (c)
        {
            case Rank.Ace: return new Vector2Int(0, 2);
            case Rank.Two: return new Vector2Int(1, 2);
            case Rank.Three: return new Vector2Int(2, 2);
            case Rank.Four: return new Vector2Int(3, 2);

            case Rank.Five: return new Vector2Int(4, 2);
            case Rank.Six: return new Vector2Int(0, 1);
            case Rank.Seven: return new Vector2Int(1, 1);
            case Rank.Eight: return new Vector2Int(2, 1);

            case Rank.Nine: return new Vector2Int(3, 1);
            case Rank.Ten: return new Vector2Int(4, 1);
            case Rank.Jack: return new Vector2Int(0, 0);
            case Rank.Queen: return new Vector2Int(1, 0);

            case Rank.King: return new Vector2Int(2, 0);
        }

        return Vector2Int.zero;
    }
}
=======
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    public enum Card
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public Card card;

    private const float UV_SCALE_X = 0.2f;
    private const float UV_SCALE_Y = 0.334f;

    private Renderer _rend;

    void OnValidate()
    {
        if (!TryGetRenderer())
            return;

        ApplyCard(_rend.sharedMaterial);
    }

    void Start()
    {
        TryGetRenderer();
        ApplyCard(_rend.material);
    }

    bool TryGetRenderer()
    {
        if (_rend != null)
            return true;

        _rend = GetComponent<Renderer>();
        return _rend != null;
    }

    void ApplyCard(Material mat)
    {
        if (mat == null) return;

        mat.SetVector("_UVScale", new Vector4(UV_SCALE_X, UV_SCALE_Y, 0, 0));

        Vector2Int grid = GetCardGridPosition(card);

        float offsetX = grid.x * UV_SCALE_X;
        float offsetY = grid.y * UV_SCALE_Y;

        mat.SetVector("_UVOffset", new Vector4(offsetX, offsetY, 0, 0));
    }

    Vector2Int GetCardGridPosition(Card c)
    {
        switch (c)
        {
            case Card.Ace: return new Vector2Int(0, 2);
            case Card.Two: return new Vector2Int(1, 2);
            case Card.Three: return new Vector2Int(2, 2);
            case Card.Four: return new Vector2Int(3, 2);

            case Card.Five: return new Vector2Int(4, 2);
            case Card.Six: return new Vector2Int(0, 1);
            case Card.Seven: return new Vector2Int(1, 1);
            case Card.Eight: return new Vector2Int(2, 1);

            case Card.Nine: return new Vector2Int(3, 1);
            case Card.Ten: return new Vector2Int(4, 1);
            case Card.Jack: return new Vector2Int(0, 0);
            case Card.Queen: return new Vector2Int(1, 0);

            case Card.King: return new Vector2Int(2, 0);
        }

        return Vector2Int.zero;
    }
}
>>>>>>> main
