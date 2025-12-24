using UnityEngine;
public class XROriginDeduplicator : MonoBehaviour
{
    [SerializeField] private string xrOriginName = "XR Origin (XR Rig)";
    [SerializeField] private string[] requiredChildren =
    {
        "Left Controller",
        "Right Controller"
    };

    private void Awake()
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        Transform[] origins = CollectOrigins(allTransforms);
        if (origins.Length <= 1)
        {
            return;
        }

        Transform keep = ChooseOrigin(origins);
        for (int i = 0; i < origins.Length; i++)
        {
            Transform origin = origins[i];
            if (origin != null && origin != keep)
            {
                origin.gameObject.SetActive(false);
            }
        }

        if (keep != null)
        {
            EnableStabilizedControllers(keep);
        }
    }

    private Transform[] CollectOrigins(Transform[] allTransforms)
    {
        int count = 0;
        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform t = allTransforms[i];
            if (t != null && t.parent == null && t.name == xrOriginName)
            {
                count++;
            }
        }

        Transform[] results = new Transform[count];
        int index = 0;
        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform t = allTransforms[i];
            if (t != null && t.parent == null && t.name == xrOriginName)
            {
                results[index++] = t;
            }
        }

        return results;
    }

    private Transform ChooseOrigin(Transform[] origins)
    {
        Transform best = origins[0];
        int bestScore = -1;

        for (int i = 0; i < origins.Length; i++)
        {
            Transform origin = origins[i];
            if (origin == null)
            {
                continue;
            }

            int score = 0;
            for (int j = 0; j < requiredChildren.Length; j++)
            {
                string childName = requiredChildren[j];
                if (!string.IsNullOrEmpty(childName) && HasChildRecursive(origin, childName))
                {
                    score++;
                }
            }

            if (score > bestScore)
            {
                bestScore = score;
                best = origin;
            }
        }

        return best;
    }

    private static bool HasChildRecursive(Transform root, string childName)
    {
        if (root == null)
        {
            return false;
        }

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            Transform child = children[i];
            if (child != null && child.name == childName)
            {
                return true;
            }
        }

        return false;
    }

    private static void EnableStabilizedControllers(Transform origin)
    {
        string[] stabilizeNames =
        {
            "Left Controller Stabilized",
            "Right Controller Stabilized"
        };

        for (int i = 0; i < stabilizeNames.Length; i++)
        {
            string name = stabilizeNames[i];
            Transform[] children = origin.GetComponentsInChildren<Transform>(true);
            for (int j = 0; j < children.Length; j++)
            {
                Transform child = children[j];
                if (child != null && child.name == name)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}
