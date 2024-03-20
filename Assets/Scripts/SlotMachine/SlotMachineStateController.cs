using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineStateController : MonoBehaviour
{
    ISlotMachineState currentState;
    List<ISlotMachineState> states;

    SlotMachine machine;

    public void Init(SlotMachine _machine)
    {
        machine = _machine;

        states = new List<ISlotMachineState>
        {
            new SlotMachineIdle(machine, this),
            new SlotMachineRolling(machine, this),
            new SlotMachineResult(machine, this),
            new SlotMachinePostResult(machine, this),
        };


        currentState = states[0];
    }

    public void ReceiveEvent(SlotMachineEvent e)
    {
        int index = currentState.OnEvent(e);

        if (index == -1)
        {
            return;
        }
        else
        {
            ChangeState(index);
        }
    }

    public void ChangeState(int index)
    {
        ChangeState(states[index]);
    }

    private void ChangeState(ISlotMachineState newState)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    public void Update()
    {
        currentState.Update();
    }
}

public enum SlotMachineEvent
{
    START,
    STOP
}

public class SlotMachineIdle : ISlotMachineState
{
    SlotMachineStateController controller;
    SlotMachine machine;

    public SlotMachineIdle(SlotMachine _machine, SlotMachineStateController _controller)
    {
        machine = _machine;
        controller = _controller;
    }

    public void OnEnter()
    {

    }

    public void OnExit()
    {

    }

    public void Update()
    {

    }

    public int OnEvent(SlotMachineEvent e)
    {
        if (e == SlotMachineEvent.START)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}

public class SlotMachineRolling : ISlotMachineState
{
    SlotMachineStateController controller;
    SlotMachine machine;

    int phase;

    public SlotMachineRolling(SlotMachine _machine, SlotMachineStateController _controller)
    {
        machine = _machine;
        controller = _controller;
    }

    public void OnEnter()
    {
        phase = 0;
    }

    public void OnExit()
    {

    }

    public void Update()
    {
        if (phase == 0)
        {
            foreach (SlotItem item in machine.slot)
            {
                item.GetComponent<RectTransform>().anchoredPosition3D += Vector3.up * 750f * Time.deltaTime;

                if (item.GetComponent<RectTransform>().anchoredPosition3D.y >= 420f)
                {
                    phase = 1;
                }
            }
        }
        else if (phase == 1)
        {
            foreach (SlotItem item in machine.slot)
            {
                item.GetComponent<RectTransform>().anchoredPosition3D += Vector3.down * 1250f * Time.deltaTime;

                if (item.GetComponent<RectTransform>().anchoredPosition3D.y <= -340f)
                {
                    item.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 340f, 0f);
                    System.Random rnd = new System.Random();
                    int rand = rnd.Next(1, 9);
                    item.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon" + rand);
                }
            }
        }
    }

    public int OnEvent(SlotMachineEvent e)
    {
        if (e == SlotMachineEvent.STOP)
        {
            return 2;
        }
        else
        {
            return -1;
        }
    }
}

public class SlotMachineResult : ISlotMachineState
{
    SlotMachineStateController controller;
    SlotMachine machine;

    bool moving; int index;

    public SlotMachineResult(SlotMachine _machine, SlotMachineStateController _controller)
    {
        machine = _machine;
        controller = _controller;
    }

    public void OnEnter()
    {
        moving = true; index = 0;
    }

    public void OnExit()
    {

    }

    public void Update()
    {
        if (moving && index <= 3)
        {
            foreach (SlotItem item in machine.slot)
            {
                item.GetComponent<RectTransform>().anchoredPosition3D += Vector3.down * 1250f * Time.deltaTime;

                if (item.GetComponent<RectTransform>().anchoredPosition3D.y <= -340f)
                {
                    item.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 340f, 0f);

                    if (index == 3)
                    {
                        moving = false;
                        controller.ChangeState(3);
                    }
                    else
                    {
                        int final = index + 1;
                        item.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon" + final);
                        index++;
                    }
                }
            }
        }
    }

    public int OnEvent(SlotMachineEvent e)
    {
        return -1;
    }
}

public class SlotMachinePostResult : ISlotMachineState
{
    SlotMachineStateController controller;
    SlotMachine machine;

    int phase;

    public SlotMachinePostResult(SlotMachine _machine, SlotMachineStateController _controller)
    {
        machine = _machine;
        controller = _controller;
    }

    public void OnEnter()
    {
        phase = 0;
    }

    public void OnExit()
    {

    }

    public void Update()
    {
        if (phase == 0)
        {
            foreach (SlotItem item in machine.slot)
            {
                item.GetComponent<RectTransform>().anchoredPosition3D += Vector3.down * 750f * Time.deltaTime;

                if (item.GetComponent<RectTransform>().anchoredPosition3D.y <= -250f)
                {
                    phase = 1;
                }
            }
        }
        else if (phase == 1)
        {
            foreach (SlotItem item in machine.slot)
            {
                item.GetComponent<RectTransform>().anchoredPosition3D += Vector3.up * 750f * Time.deltaTime;

                if (item.GetComponent<RectTransform>().anchoredPosition3D.y >= 340f)
                {
                    item.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 340f, 0f);
                    controller.ChangeState(0);
                }
            }
        }
    }

    public int OnEvent(SlotMachineEvent e)
    {
        return -1;
    }
}

public interface ISlotMachineState
{
    public void OnEnter();

    public void Update();

    public void OnExit();

    public int OnEvent(SlotMachineEvent e);
}
