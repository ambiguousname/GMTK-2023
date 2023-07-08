using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class StageTimeline : MonoBehaviour
{
    private const string ActionLineRegex = @"(%\w+%)*(\w+):(\w+)\|(.*)";
    private struct StageAction {
        public string type;
        public string objName;
        public List<string> args;
        public string uniqueActionID;
        public StageAction(string actionLine) {
            var actionMatch = Regex.Match(actionLine, ActionLineRegex);
            var actionInfo = actionMatch.Groups;

            uniqueActionID = actionInfo[1].Value.Replace("%","");

            type = actionInfo[2].Value;

            objName = actionInfo[3].Value;

            args = new List<string>();


            if (actionInfo.Count > 4) {
                var actionArgs = actionInfo[4].Value.Split("|");
                foreach (var arg in actionArgs) {
                    if (arg.Length > 0) {
                        args.Add(arg);
                    }
                }
            }
        }
    }

    public TextAsset timelineToRead;
    private List<List<StageAction>> actions;
    Dictionary<string, UnityEvent> onUniqueAction; 

    int currTime = -1;
    private void Awake() {
        actions = new List<List<StageAction>>();
        onUniqueAction = new Dictionary<string, UnityEvent>();
        ReadTimelineFile(timelineToRead.text);
        currTime = -1;
    }

    /*
     * Ideal formatting:
     * ACTION TEXT
     * ACTION TEXT
     * ACTION TEXT
     * --
     * ^ This is a separator for the actions, so it represents one turn.
     */
    private void ReadTimelineFile(string text) {
        var lines = text.Replace("\r", "").Split("--");
        for (int turn = 0; turn < lines.Length; turn++) {
            // Get rid of carriage return
            var actionText = lines[turn].Split("\n");
            List<StageAction> actionsToExecute = new List<StageAction>();
            foreach (var action in actionText) {
                // Not just whitespace.
                if (action.Length > 0 && Regex.IsMatch(action, @"^(?!\s+$).*")) {
                    actionsToExecute.Add(new StageAction(action));
                }
            }

            actions.Add(actionsToExecute);
        }
    }

    public void AddOnUniqueAction(string actionName, UnityAction function) {
        if (onUniqueAction.ContainsKey(actionName)) {
            onUniqueAction[actionName].AddListener(function);
        } else {
            var unityEvent = new UnityEvent();
            unityEvent.AddListener(function);
            onUniqueAction.Add(actionName, unityEvent);
        }
    }

    public void Advance() {
        currTime++;
        if (currTime >= actions.Count) {
            return;
        }
        var currActionList = actions[currTime];
        if (currActionList.Count <= 0) {
            return;
        }
        foreach (var action in currActionList) {
            Debug.Log(action.uniqueActionID);
            if (action.uniqueActionID.Length > 0 && onUniqueAction.ContainsKey(action.uniqueActionID)) {
                Debug.Log(action.uniqueActionID + " overridden.");
                onUniqueAction[action.uniqueActionID].Invoke();
                continue;
            }
            var referenced = GameObject.Find(action.objName);
            if (referenced == null) {
                Debug.LogWarning("Could not find " + action.objName + " for turn " + currTime);
                continue;
            }

            if (referenced.TryGetComponent(out GridObject component)) {
                System.Type refType = component.GetType();
                var methods = refType.GetMethods();
                foreach (var method in methods) {
                    if (method.Name == action.type) {
                        Debug.Log(method.Name);
                        foreach (var arg in action.args) {
                            Debug.Log(arg);
                        }
                        method.Invoke(component, action.args.ToArray());
                        break;
                    }
                }
            }
        }
    }

}
