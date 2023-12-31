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
    private List<List<StageAction>> timelineCopy;
    Dictionary<string, UnityEvent> onUniqueAction; 

    int currTime = -1;

    Stage stage;
    private void Awake() {
        actions = new List<List<StageAction>>();
        onUniqueAction = new Dictionary<string, UnityEvent>();
        ReadTimelineFile(timelineToRead.text);
        timelineCopy = new List<List<StageAction>>(actions);
        currTime = -1;

        stage = GetComponent<Stage>();
    }
    public void ResetTimeline() {
        actions.Clear();
        actions = new List<List<StageAction>>(timelineCopy);
        currTime = -1;
        onUniqueAction.Clear();
        onUniqueAction = new Dictionary<string, UnityEvent>();
    }

    /*
     * Ideal formatting:
     * ACTION TEXT
     * ACTION TEXT
     * ACTION TEXT
     * --
     * ^ This is a separator for the actions, so it represents one turn.
     */
    private void ReadTimelineFile(string text, int startIndex=0) {
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

            if (actions.Count <= turn + startIndex) {
                actions.Add(actionsToExecute);
            } else {
                // For appending multiple timelines to the list of actions:
                actions[turn + startIndex].AddRange(actionsToExecute);
            }
        }
    }

    public void RemoveAllReferencesToObject(string objectName) {
        foreach (var actionsList in actions) {
            for (int i = 0; i < actionsList.Count; i++) {
                var action = actionsList[i];
                if (action.objName == objectName) {
                    actionsList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void AppendTimeline(string text) {
        ReadTimelineFile(text, currTime);
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
            GameObject.Find("Player").GetComponent<Player>().NextNight();
            return;
        }
        var currActionList = actions[currTime];
        if (currActionList.Count <= 0) {
            return;
        }
        var source = GameObject.Find("TurnSound").GetComponent<AudioSource>();
        source.pitch = Random.Range(0.8f, 1.2f);
        source.Play();
        foreach (var action in currActionList) {
            //Debug.Log(action.uniqueActionID);
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
                    bool isAllStringParams = true;
                    foreach (var param in method.GetParameters()) {
                        if (param.ParameterType != typeof(string)) {
                            isAllStringParams = false;
                            break;
                        }
                    }
                    if (method.Name == action.type && isAllStringParams) {
                        /*Debug.Log(method.Name);
                        foreach (var arg in action.args) {
                            Debug.Log(arg);
                        }*/
                        method.Invoke(component, action.args.ToArray());
                        break;
                    }
                }
            }
        }
    }

}
