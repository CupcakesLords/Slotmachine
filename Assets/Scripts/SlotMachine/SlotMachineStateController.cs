using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineStateController : MonoBehaviour
{
    ISlotMachineState currentState;
    Dictionary<SlotMachineState ,ISlotMachineState> states;

    SlotMachine machine;

    public void Init(SlotMachine _machine)
    {
        machine = _machine;

        states = new Dictionary<SlotMachineState, ISlotMachineState>
        {
            { SlotMachineState.IDLE, new SlotMachineIdle(machine, this) },
            { SlotMachineState.ROLLING, new SlotMachineRolling(machine, this) },
            { SlotMachineState.RESULT, new SlotMachineResult(machine, this) },
            { SlotMachineState.POST_RESULT, new SlotMachinePostResult(machine, this) }
        };

        currentState = states[SlotMachineState.IDLE];
    }

    public void ReceiveEvent(SlotMachineEvent e, object data)
    {
        SlotMachineState index = currentState.OnEvent(e, data);

        if (index == SlotMachineState.NONE)
        {
            return;
        }
        else
        {
            ChangeState(index, data);
        }
    }

    public void ChangeState(SlotMachineState index, object data)
    {
        ChangeState(states[index], data);
    }

    private void ChangeState(ISlotMachineState newState, object data)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(data);
    }

    public void Update()
    {
        currentState.Update();
    }

    public ISlotMachineState GetState()
    {
        return currentState;
    }
}

public enum SlotMachineEvent
{
    START,
    STOP
}

public enum SlotMachineState
{
    NONE,
    IDLE,
    ROLLING,
    RESULT,
    POST_RESULT
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

    public void OnEnter(object data)
    {

    }

    public void OnExit()
    {

    }

    public void Update()
    {

    }

    public SlotMachineState OnEvent(SlotMachineEvent e, object data)
    {
        if (e == SlotMachineEvent.START)
        {
            return SlotMachineState.ROLLING;
        }
        else
        {
            return SlotMachineState.NONE;
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

    public void OnEnter(object data)
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

    public SlotMachineState OnEvent(SlotMachineEvent e, object data)
    {
        if (e == SlotMachineEvent.STOP)
        {
            return SlotMachineState.RESULT;
        }
        else
        {
            return SlotMachineState.NONE;
        }
    }
}

public class SlotMachineResult : ISlotMachineState
{
    SlotMachineStateController controller;
    SlotMachine machine;

    bool moving; int index;

    List<int> result;

    public SlotMachineResult(SlotMachine _machine, SlotMachineStateController _controller)
    {
        machine = _machine;
        controller = _controller;
    }

    public void OnEnter(object data)
    {
        moving = true; index = 0;

        if (data != null)
        {
            result = (List<int>)data;
        }
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
                        controller.ChangeState(SlotMachineState.POST_RESULT, null);
                    }
                    else
                    {
                        int final = result[3 - index - 1];
                        item.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon" + final);
                        index++;
                    }
                }
            }
        }
    }

    public SlotMachineState OnEvent(SlotMachineEvent e, object data)
    {
        return SlotMachineState.NONE;
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

    public void OnEnter(object data)
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
                    controller.ChangeState(SlotMachineState.IDLE, null);
                }
            }
        }
    }

    public SlotMachineState OnEvent(SlotMachineEvent e, object data)
    {
        return SlotMachineState.NONE;
    }
}

public interface ISlotMachineState
{
    public void OnEnter(object data);

    public void Update();

    public void OnExit();

    public SlotMachineState OnEvent(SlotMachineEvent e, object data);
}
