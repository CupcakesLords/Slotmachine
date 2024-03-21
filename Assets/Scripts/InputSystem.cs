using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSystem : MonoBehaviour
{
    public SlotMachineStateController slotmachineController;
    public SlotMachine slotmachine;

    public Image[] choices;

    List<int> input = new List<int>();

    private void Start()
    {
        slotmachineController.Init(slotmachine);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            foreach (Image image in choices)
            {
                image.enabled = false;
            }
            slotmachineController.ReceiveEvent(SlotMachineEvent.START, null);
        }
    }

    public void ButtonChooseSlot(int index)
    {
        if (slotmachineController.GetState() is SlotMachineRolling)
        {
            if (input.Count < 3)
            {
                input.Add(index);

                choices[input.Count - 1].sprite = Resources.Load<Sprite>("icon" + index);
                choices[input.Count - 1].enabled = true;

                if (input.Count == 3)
                {
                    slotmachineController.ReceiveEvent(SlotMachineEvent.STOP, new List<int> { input[0], input[1], input[2] });
                    input.Clear();
                }
            }
        }
        else
        {
            Debug.Log("Input only allowed during rolling");
        }
    }
}
