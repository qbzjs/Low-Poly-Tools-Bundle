using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoDisplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buildingNameText;
    [SerializeField] TextMeshProUGUI buildingTypeText;

    [SerializeField] FourColorFillTextBar capacityUnitDisplay;
    [SerializeField] FourColorFillTextBar capacityMassDisplay;
    [SerializeField] FourColorFillTextBar capacityVolumeDisplay;
    [SerializeField] GoodDisplayer goodDisplayerPrefab;
    List<GoodDisplayer> goodDisplayers = new List<GoodDisplayer>();

    public void DisplayInformation(Building building)
    {
        if (goodDisplayers.Count > 0)
        {
            for (int i = goodDisplayers.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(goodDisplayers[i].gameObject);
                goodDisplayers.RemoveAt(i);
            }
        }
        buildingNameText.text = building.buildingName;
        buildingTypeText.text = building.buildingModel.name;
        List<StackDisplay> relevantStacks = building.GetRelevantStacks();
        if (relevantStacks == null) return;

        int count = 0;
        foreach (StackDisplay stack in relevantStacks)
        {
            GoodDisplayer newGoodDisplayer = Instantiate(goodDisplayerPrefab, this.transform);
            newGoodDisplayer.DisplayInformation(stack);
            RectTransform rectTransform = newGoodDisplayer.GetComponent<RectTransform>();
            if (count % 2 == 1)
            {
                rectTransform.position += new Vector3(rectTransform.sizeDelta.x, (count - 1) * 0.5f * -rectTransform.sizeDelta.y, 0);
            }
            else
            {
                rectTransform.position += new Vector3(0, count * 0.5f * -rectTransform.sizeDelta.y, 0);
            }
            count++;
            goodDisplayers.Add(newGoodDisplayer);
        }

        Capacity capacity = building.GetCapacity();
        Inventory buildingInventory = building.GetInventory();
        Capacity freeCapacity = buildingInventory.GetFreeCapacity();
        Capacity reservedCapacity = buildingInventory.GetReservedCapacity();
        Capacity tempOccupiedCapacity = buildingInventory.GetTemporarilyOccupiedCapacity();

        Capacity reserveFilledCapacity = capacity - freeCapacity;//capacity after only active depositing logistic changes
        Capacity tempFilledCapacity = capacity - (freeCapacity + reservedCapacity);//capacity currently in the inventory
        Capacity filledCapacity = capacity - (freeCapacity + reservedCapacity + tempOccupiedCapacity);//capacity after only active withdrawing logistic changes
        Capacity reserveCapacities = reservedCapacity - tempOccupiedCapacity;

        if (capacity.unitCapacity > 0)
        {
            capacityUnitDisplay.gameObject.SetActive(true);
            string unitText;
            if (reserveCapacities.unitCapacity > 0)
            {
                unitText = filledCapacity.unitCapacity + " (+" + reserveCapacities.unitCapacity + ") / " + capacity.unitCapacity;
            }
            else
            {
                unitText = filledCapacity.unitCapacity + " (" + reserveCapacities.unitCapacity + ") / " + capacity.unitCapacity;
            }
            capacityUnitDisplay.UpdateBar(
            (float)reserveFilledCapacity.unitCapacity / capacity.unitCapacity,
            (float)tempFilledCapacity.unitCapacity / capacity.unitCapacity,
            (float)filledCapacity.unitCapacity / capacity.unitCapacity,
            unitText);
        }
        else
        {
            capacityUnitDisplay.gameObject.SetActive(false);
        }

        if (capacity.massCapacity > 0)
        {
            capacityMassDisplay.gameObject.SetActive(true);
            string unitText;
            if (reserveCapacities.massCapacity > 0)
            {
                unitText = Math.Round(filledCapacity.massCapacity,1) + " (+" + Math.Round(reserveCapacities.massCapacity, 1) + ") / " + capacity.massCapacity;
            }
            else
            {
                unitText = Math.Round(filledCapacity.massCapacity, 1) + " (" + Math.Round(reserveCapacities.massCapacity, 1) + ") / " + capacity.massCapacity;
            }
            capacityMassDisplay.UpdateBar(
            reserveFilledCapacity.massCapacity / capacity.massCapacity,
            tempFilledCapacity.massCapacity / capacity.massCapacity,
            filledCapacity.massCapacity / capacity.massCapacity,
            unitText);
        }
        else
        {
            capacityMassDisplay.gameObject.SetActive(false);
        }

        if (capacity.volumeCapacity > 0)
        {
            capacityVolumeDisplay.gameObject.SetActive(true);
            string unitText;
            if (reserveCapacities.volumeCapacity > 0)
            {
                unitText = Math.Round(filledCapacity.volumeCapacity, 1) + " (+" + Math.Round(reserveCapacities.volumeCapacity, 1) + ") / " + capacity.volumeCapacity;
            }
            else
            {
                unitText = Math.Round(filledCapacity.volumeCapacity, 1) + " (" + Math.Round(reserveCapacities.volumeCapacity, 1) + ") / " + capacity.volumeCapacity;
            }
            capacityVolumeDisplay.UpdateBar(
            reserveFilledCapacity.volumeCapacity / capacity.volumeCapacity,
            tempFilledCapacity.volumeCapacity / capacity.volumeCapacity,
            filledCapacity.volumeCapacity / capacity.volumeCapacity,
            unitText);
        }
        else
        {
            capacityVolumeDisplay.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (goodDisplayers.Count > 0)
        {
            for (int i = goodDisplayers.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(goodDisplayers[i].gameObject);
                goodDisplayers.RemoveAt(i);
            }
        }
    }
}
