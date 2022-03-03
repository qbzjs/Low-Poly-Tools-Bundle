using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionPlacementManager : MonoBehaviour
{
    static ConstructionPlacementManager instance;

    [SerializeField] BuildingSpaceHolder buildingSpaceHolder;
    NavMeshManager navMeshManager;
    Building prefab;
    public ConstructionSpace constructionSpace;
    [SerializeField] Building currentBuilding;
    Vector3 modulePos = Vector3.zero;

    public bool isBuilding
    {
        get
        {
            return prefab != null;
        }
    }

    public static ConstructionPlacementManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        navMeshManager = NavMeshManager.GetInstance();
    }

    void Update()
    {
        if (Input.GetButtonUp("Fire2") && prefab != null && !EventSystem.current.IsPointerOverGameObject())
        {
            SelectBuildingType(null);
        }
        if (Input.GetButtonUp("Rotate Building Clockwise") && prefab != null)
        {
            Rotate(1);
        }
        else if (Input.GetButtonUp("Rotate Building Counterclockwise") && prefab != null)
        {
            Rotate(-1);
        }
        if (prefab != null)
        {

            Vector3 hitPoint = Vector3.zero;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.down, new Vector3(0, 2.27f, 0));
            float ent = 100.0f;
            if (plane.Raycast(ray, out ent))
            {
                hitPoint = ray.GetPoint(ent);

                Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * ent, Color.red);
                return;
            }

            Vector3 gridPos = new Vector3(Mathf.RoundToInt(hitPoint.x * WorldConsts.GRID_SIZE_RECIPROCAL) * WorldConsts.GRID_SIZE, WorldConsts.BUILDING_HIGHT, Mathf.RoundToInt(hitPoint.z * WorldConsts.GRID_SIZE_RECIPROCAL) * WorldConsts.GRID_SIZE);

            Vector3 newModulePos = gridPos;

            if (newModulePos == modulePos)
            {
                if (!constructionSpace.isBlocked && Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
                {
                    PlaceBuilding();
                }
                return;
            }

            modulePos = newModulePos;

            currentBuilding.transform.position = gridPos;

            if (!constructionSpace.isBlocked && Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceBuilding();
            }
        }

    }

    public void ToggleBuildingMode(bool destroy = true)
    {
        buildingSpaceHolder.ToggleBuildingSpaces();

        if (!isBuilding)
        {
            SelectBuildingType(null);
        }
    }

    public void SelectBuildingType(Building constructionInfo)
    {
        prefab = constructionInfo;
        if (currentBuilding != null)
        {
            GameObject.Destroy(currentBuilding.gameObject);
        }
        if (prefab == null)
        {
            constructionSpace = null;
            return;
        }
        currentBuilding = Instantiate<Building>(prefab);
        constructionSpace = currentBuilding.GetConstructionSpace();
    }

    public void PlaceBuilding()
    {
        currentBuilding.transform.position -= new Vector3(0, 0.01f, 0);//prevents clipping with new buildings
        //destroy ridgidbodys to avoid collision triggers and move to buildingSpaceHolder
        UpgradeSpace upgradeSpace = currentBuilding.GetUpgradeSpace();
        Destroy(constructionSpace.gameObject.GetComponent<Rigidbody>());
        Destroy(upgradeSpace.gameObject.GetComponent<Rigidbody>());
        buildingSpaceHolder.AddBuildingSpaces(new BuildingSpace[2] { constructionSpace, upgradeSpace });
        //get and scale Raycast Target
        BoxCollider raycastTarget = currentBuilding.GetComponent<BoxCollider>();
        int buildingHight = 5;//TODO remove hardecoded number
        raycastTarget.center = new Vector3(0, buildingHight * 0.5f, 0);
        raycastTarget.size = new Vector3(constructionSpace.transform.localScale.x, buildingHight, constructionSpace.transform.localScale.z);
        //Add Building to NavMeshHolder and Update NavMesh
        currentBuilding.transform.SetParent(navMeshManager.transform);
        //start building
        currentBuilding.StartConstruction();
        //reset current building..
        currentBuilding = null;
        //..and building selection
        SelectBuildingType(null);
    }

    public void Rotate(int direction)
    {
        currentBuilding.transform.rotation = Quaternion.Euler(currentBuilding.transform.rotation.eulerAngles + WorldConsts.STANDARD_ROTATION * direction);
    }
}