using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class StageTimeline : MonoBehaviour
{
    private const string ActionLineRegex = @"(\w+):(\w+)|(.*)";
    private struct StageAction {
        public string type;
        public string objName;
        public List<string> args;
        public StageAction(string actionLine) {
            MatchCollection matches = Regex.Matches(actionLine, ActionLineRegex);
            

            var actionInfo = matches[0].Groups;
            type = actionInfo[1].Value;

            objName = actionInfo[2].Value;

            args = new List<string>();
            if (matches.Count > 1) {
                var actionArgs = matches[1].Value.Split("|");
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

    int currTime = -1;
    private void Awake() {
        actions = new List<List<StageAction>>();
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
                        method.Invoke(component, action.args.ToArray());
                        break;
                    }
                }
            }
        }
    }

}
