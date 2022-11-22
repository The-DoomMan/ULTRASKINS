using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ButtonManager : MonoBehaviour
{
    RectTransform ToggleBorder;
    RectTransform buttonT;
    RectTransform borderT;
    RectTransform TextT;
    Image Image;
    Text Text;

    public enum AlignmentX
    {
        Left,
        Center,
        Right
    };

    public enum AlignmentY
    {
        Top,
        Middle,
        Bottom
    };

    public enum ButtonType
    {
        Normal,
        Toggle,
        Image
    };

    public ButtonType buttonType = ButtonType.Normal;

    public Font font;
    public string buttonText = "Text";
    public Sprite imageSprite;

    public AlignmentX alignmentX = AlignmentX.Left;
    public AlignmentY alignmentY = AlignmentY.Middle;

    public float Widght = 160;
    public float Height = 30;

    public int FontSize = 16;

    public float toggleWidght = 25;
    public bool isEnabled;

    void Start()
    {
        buttonT = transform.GetComponent<RectTransform>();
        borderT = transform.GetComponentsInChildren<RectTransform>()[1];
        TextT = transform.GetComponentsInChildren<RectTransform>()[2];
        Text = transform.GetComponentInChildren<Text>();
        if(transform.GetComponentsInChildren<RectTransform>().Length > 3)
        ToggleBorder = transform.GetComponentsInChildren<RectTransform>()[3];
        if (transform.GetComponentsInChildren<Image>().Length > 2)
            Image = transform.GetComponentsInChildren<Image>()[2];
    }

    void Update()
    {
        buttonT.sizeDelta = new Vector2(Widght, Height);
        borderT.sizeDelta = new Vector2(Widght, Height);
        TextT.sizeDelta = new Vector2(Widght, Height);
        Text.font = font;
        if (ToggleBorder && buttonType == ButtonType.Toggle)
        {
            ToggleBorder.transform.position = transform.position + new Vector3((Widght - ToggleBorder.sizeDelta.x)/2, 0, 0);
            ToggleBorder.sizeDelta = new Vector2(toggleWidght, transform.GetComponent<RectTransform>().sizeDelta.y/1.5f);
            ToggleBorder.GetComponent<Image>().fillCenter = isEnabled;
        }
        if(Image && buttonType == ButtonType.Image)
        {
            Image.sprite = imageSprite;
            Image.GetComponent<RectTransform>().sizeDelta = new Vector2(Height, Height);
            Image.transform.position = transform.position + new Vector3((Widght - Height)/2, 0, 0);
        }
        TextAnchor TA;
        switch(alignmentX)
        {
            case AlignmentX.Left:
                switch (alignmentY)
                {
                    case AlignmentY.Top:
                        TA = TextAnchor.UpperLeft;
                        break;
                    case AlignmentY.Middle:
                        TA = TextAnchor.MiddleLeft;
                        break;
                    default:
                        TA = TextAnchor.LowerLeft;
                        break;
                }
                break;
            case AlignmentX.Center:
                switch (alignmentY)
                {
                    case AlignmentY.Top:
                        TA = TextAnchor.UpperCenter;
                        break;
                    case AlignmentY.Middle:
                        TA = TextAnchor.MiddleCenter;
                        break;
                    default:
                        TA = TextAnchor.LowerCenter;
                        break;
                }
                break;
            default:
                switch (alignmentY)
                {
                    case AlignmentY.Top:
                        TA = TextAnchor.UpperRight;
                        break;
                    case AlignmentY.Middle:
                        TA = TextAnchor.MiddleRight;
                        break;
                    default:
                        TA = TextAnchor.LowerRight;
                        break;
                }
                break;
        }
        Text.fontSize = FontSize;
        Text.alignment = TA;
        Text.text = buttonText;
    }
}