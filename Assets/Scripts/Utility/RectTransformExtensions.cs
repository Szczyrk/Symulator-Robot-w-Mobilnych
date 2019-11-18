using UnityEngine;

public static class RectTransformExtensions
{

    public static void SetPositionOfPivot(this RectTransform transformRect, Vector2 newPos)
    {
        transformRect.localPosition = new Vector3(newPos.x, newPos.y, transformRect.localPosition.z);
    }

    public static void SetSize(this RectTransform transformRect, Vector2 newSize)
    {
        Vector2 oldSize = transformRect.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        transformRect.offsetMin = transformRect.offsetMin - new Vector2(deltaSize.x * transformRect.pivot.x, deltaSize.y * transformRect.pivot.y);
        transformRect.offsetMax = transformRect.offsetMax + new Vector2(deltaSize.x * (1f - transformRect.pivot.x), deltaSize.y * (1f - transformRect.pivot.y));
    }
}
