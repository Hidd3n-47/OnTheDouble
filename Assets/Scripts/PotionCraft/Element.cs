using UnityEngine;

public enum Element
{
    Ignis = 0, 
    Aether,
    Root,
    Dew,
    Spark,
    Max
}

public static class ElementUtils
{
    // Added so if we change an element name we can keep underlying enums intact, and 
    // can support multi-word names if desired
    public static string ElementToName(Element element)
    {
        // That said, we don't need it yet \_(ツ)_/
        return element.ToString();
    }

    // For consistency
    public static Color ElementToColour(Element element)
    {
        switch (element)
        {
            case Element.Ignis:
                return Color.darkRed;
            case Element.Aether:
                return Color.antiqueWhite;
            case Element.Root:
                return Color.burlywood;
            case Element.Dew:
                return Color.aquamarine;
            case Element.Spark:
                return Color.lightGoldenRodYellow;    
        }
        return Color.black;
    }
}
