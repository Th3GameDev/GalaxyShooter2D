using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwap : MonoBehaviour
{
    private Animator _anim;

    private int _iD;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();

        SwapSprite();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void SwapSprite()
    {
        int randomNum = Random.Range(1, 7);

        _iD = randomNum;

        switch (_iD)
        {
            case 1:
                _anim.SetBool("IsAmmoGlitch", true);
                break;

            case 2:
                _anim.SetBool("IsGLGlitch", true);
                break;

            case 3:
                _anim.SetBool("IsTSGlitch", true);
                break;

            case 4:
                _anim.SetBool("IsSpeedGlitch", true);
                break;

            case 5:
                _anim.SetBool("IsShieldGlitch", true);
                break;

            case 6:
                _anim.SetBool("IsRepairGlitch", true);
                break;

            default:
                break;
        }
    }
}
