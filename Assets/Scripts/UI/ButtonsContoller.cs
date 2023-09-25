using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonsContoller : MonoBehaviour
{
    public RenSharpProcessor RenSharp;

	public GameObject DrinkButtons;
    public GameObject JohnLateButtons;
    public GameObject WhyMe;
    public GameObject WhyMe2;
    public GameObject ElizDialog;
    public GameObject SkivDialog;

    public GameObject bad_company1;
    public GameObject bad_company2;
    public GameObject bad_company3;

    public GameObject fake1;
    public GameObject fake2;
    public GameObject fake4;
    public GameObject fake8;

    public GameObject afterOperation;

	public void EnableButton(string buttonName)
    {
        if (buttonName == "drink")
            DrinkButtons.SetActive(true);
        else if (buttonName == "johnlate")
            JohnLateButtons.SetActive(true);
        else if (buttonName == "WhyMe")
            WhyMe.SetActive(true);
        else if (buttonName == "WhyMe2")
            WhyMe2.SetActive(true);
        else if (buttonName == "ElizDialog")
            ElizDialog.SetActive(true);
        else if (buttonName == "SkivDialog")
            SkivDialog.SetActive(true);
        else if (buttonName == "bad_company1")
            bad_company1.SetActive(true);
        else if (buttonName == "bad_company2")
            bad_company2.SetActive(true);
        else if (buttonName == "bad_company3")
            bad_company3.SetActive(true);
        else if (buttonName == "fake1")
            fake1.SetActive(true);
		else if (buttonName == "fake2")
			fake2.SetActive(true);
		else if (buttonName == "fake4")
			fake4.SetActive(true);
		else if (buttonName == "fake8")
        {
			ShuffleButtons(fake8);
			fake8.SetActive(true);
		}
        else if(buttonName == "afterOperation")
        {
            afterOperation.SetActive(true);
        }
		else
        {
            Debug.LogError("NO BUTTON WAS FOUND!");
            RenSharp.IsPaused = false;
        }
	}

	public void ButtonClick(string label)
	{
        bool isFound = true;

        Debug.Log("BUTTON LABEL: " + label);

		if (label == "Whiskey")
        {
            DrinkButtons.SetActive(false);
        }
        else if(label == "DrinkNoCare")
        {
            DrinkButtons.SetActive(false);
        }
        // John late
        else if(label == "CompanyIsImportant" || label == "NoProblemJohn")
        {
            JohnLateButtons.SetActive(false);
        }
        // Offer
        else if(label == "acceptOffer")
        {
            WhyMe.SetActive(false);
            WhyMe2.SetActive(false);
		}
        else if(label == "WhyMe")
        {
            WhyMe.SetActive(false);
        }
        else if(label == "declineOffer")
        {
            WhyMe2.SetActive(false);
        }
        // Eliz Dialog
        else if(label == "ElizToxic" || label == "AreYouSpying" || label == "ElizContinue")
        {
            ElizDialog.SetActive(false);
        }
        // Skiv Dialog
        else if(label == "DictaphoneQuestion" || label == "SkivSilence")
        {
            SkivDialog.SetActive(false);
        }
        // Bad company 1
        else if(label == "no_try" || label == "bad_company1")
        {
            bad_company1.SetActive(false);
        }
		// Bad company 2
		else if (label == "bad_company2")
		{
			bad_company2.SetActive(false);
		}
		// Bad company 2 and 3
		else if (label == "bad_company1_continue" || label == "bad_company3")
		{
			bad_company1.SetActive(false);
			bad_company2.SetActive(false);
			bad_company3.SetActive(false);
		}
        // Fake buttons
        else if(label == "fake2")
        {
            fake1.SetActive(false);
        }
        else if(label == "fake4")
        {
			fake2.SetActive(false);
		}
		else if(label == "fake8")
        {
			fake4.SetActive(false);
		}
        else if(label == "fakeChoiceContinue")
        {
            fake8.SetActive(false);
        }
        else if(label == "deathToCompany" || label == "nothingMatters" || label == "shyDialog")
        {
            afterOperation.SetActive(false);
        }
		// Else
		else
        {
            Debug.LogError(" ÕŒœ ¿ Õ≈ Õ¿…ƒ≈Õ¿: " + label);
            isFound = false;
        }

        if (isFound)
            RenSharp.GotoLabel(label);
        RenSharp.ProccessCommandAfterPause();
	}

    private void ShuffleButtons(GameObject parent)
    {
        if (parent == null)
            return;

        List<GameObject> children = new List<GameObject>();

        foreach(Transform obj in parent.transform)
        {
            children.Add(obj.gameObject);
        }

        children = children.OrderBy(a => Random.value).ToList();

        try
        {
			children[0].transform.localPosition = new Vector3(x: 0, y: -180, z: 0);
			children[1].transform.localPosition = new Vector3(x: 0, y: -90, z: 0);
			children[2].transform.localPosition = new Vector3(x: 0, y: 0, z: 0);
			children[3].transform.localPosition = new Vector3(x: 0, y: 90, z: 0);
			children[4].transform.localPosition = new Vector3(x: 300, y: -180, z: 0);
			children[5].transform.localPosition = new Vector3(x: 300, y: -90, z: 0);
			children[6].transform.localPosition = new Vector3(x: 300, y: 0, z: 0);
			children[7].transform.localPosition = new Vector3(x: 300, y: 90, z: 0);
			children[8].transform.localPosition = new Vector3(x: -300, y: -180, z: 0);
			children[9].transform.localPosition = new Vector3(x: -300, y: -90, z: 0);
			children[10].transform.localPosition = new Vector3(x: -300, y: 0, z: 0);
			children[11].transform.localPosition = new Vector3(x: -300, y: 90, z: 0);
		}
        catch
        {
            Debug.LogError("NOT ALL BUTTONS SHUFFLED");
        }
        
	}

}
