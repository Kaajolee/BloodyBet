using System.Collections.Generic;
using UnityEngine;

public class Horizontal3DLayout : MonoBehaviour
{
    [SerializeField] private float spacing = 2f;      // Distance between cards
    [SerializeField] private float lerpSpeed = 5f;    // How fast cards move into place

    private readonly List<Transform> cards = new List<Transform>();
    private readonly List<Vector3> targetPositions = new List<Vector3>();

    public void AddCardInstance(GameObject cardObj)
    {
        cards.Add(cardObj.transform);
        UpdateTargetPositions();
    }

    public void RemoveCard(GameObject cardObj)
    {
        cards.Remove(cardObj.transform);
        Destroy(cardObj);
        UpdateTargetPositions();
    }
    public void ClearCards()
    {
        foreach (Transform card in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
        targetPositions.Clear();
    }

    private void UpdateTargetPositions()
    {
        targetPositions.Clear();

        if (cards.Count == 0)
            return;

        float totalWidth = (cards.Count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < cards.Count; i++)
        {
            Vector3 targetPos = new Vector3(startX + i * spacing, 0f, 0f);
            targetPositions.Add(targetPos);
        }
    }

    private void Update()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (i >= targetPositions.Count) continue;

            // Smoothly move toward target position
            cards[i].localPosition = Vector3.Lerp(
                cards[i].localPosition,
                targetPositions[i],
                Time.deltaTime * lerpSpeed
            );
        }
    }
}
