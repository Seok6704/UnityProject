using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JamesAlone : MonoBehaviour
{
    GameObject Dialog;
    PlayerController_v3 pc;
    bool flag;

    void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController_v3>();
        Dialog = GameObject.Find("Panel_Dialog");
        Invoke("OnJamesDialogue", 0.2f);
    }

    void OnJamesDialogue()
    {
        flag = true;
        pc.ChangeisOn();
        Dialog.GetComponent<DialoguesManager>().SetDialogue(801, 0);
        Dialog.GetComponent<UI_Mover>().SetPos2Parent();
    }

    public void OffJamesDialouge()
    {
        if(flag) 
        {
            flag = false;
            pc.ChangeisOn();
        }
        else return;
    }

}