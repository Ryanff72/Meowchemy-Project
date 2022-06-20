using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCanvasFlipHelper : MonoBehaviour
{
    GameObject playerSpriteParent;
    // Start is called before the first frame update
    void Start()
    {
        playerSpriteParent = GameObject.Find("CatSpriteParent");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerSpriteParent.transform.localScale.x > 0)
        {
            transform.localScale = new Vector2(0.03881681f, 0.03881681f);
            transform.localPosition = new Vector2(2.43f, 21.1f);
        }
        else
        {
            transform.localScale = new Vector2(-0.03881681f, 0.03881681f);
            transform.localPosition = new Vector2(-5.1f, 21.1f);
        }
    }
}
