using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    //public SlotItem[] slot;
    //private bool moving;
    //private bool stop;

    //private void Start()
    //{
    //    moving = false;
    //    stop = false;

    //    foreach (SlotItem item in slot)
    //    {
    //        Debug.Log(item.gameObject.GetComponent<RectTransform>().anchoredPosition3D);
    //    }
    //}

    public SlotMachineStateController slotmachineController;
    public SlotMachine slotmachine;

    private void Start()
    {
        slotmachineController.Init(slotmachine);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            slotmachineController.ReceiveEvent(SlotMachineEvent.START);
        }

        if (Input.GetKeyDown("a"))
        {
            slotmachineController.ReceiveEvent(SlotMachineEvent.STOP);
        }
    }

    //private void LateUpdate()
    //{
    //    UpdatePosition();
    //}

    //void UpdatePosition()
    //{
    //    if (moving)
    //    {
    //        foreach (SlotItem item in slot)
    //        {
    //            item.GetComponent<RectTransform>().anchoredPosition3D += Vector3.down * 1250f * Time.deltaTime;

    //            if (item.GetComponent<RectTransform>().anchoredPosition3D.y <= -340f)
    //            {
    //                item.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 340f, 0f);

    //                if (stop)
    //                {
    //                    moving = false;
    //                }
    //            }
    //        }
    //    }
    //}
}
