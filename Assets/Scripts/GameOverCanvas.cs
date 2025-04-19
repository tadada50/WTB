using UnityEngine;
using UnityEngine.UI;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] RawImage leftSideImage;
    [SerializeField] RawImage rightSideImage;
    [SerializeField] Texture leftSideWintexture;
    [SerializeField] Texture rightSideWinTexture;
    [SerializeField] Texture leftSideLoseTexture;
    [SerializeField] Texture rightSideLoseTexture;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetWinner(bool isLeftPlayerWinner)
    {
        if (isLeftPlayerWinner)
        {
            leftSideImage.texture = leftSideWintexture;
            rightSideImage.texture = rightSideLoseTexture;
        }
        else
        {
            leftSideImage.texture = leftSideLoseTexture;
            rightSideImage.texture = rightSideWinTexture;
        }
    }
}
