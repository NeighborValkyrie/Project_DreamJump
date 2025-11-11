using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockOutKit
{
    [ExecuteInEditMode]
    public class PrimitiveGenerator : MonoBehaviour
    {
#if UNITY_EDITOR
        [ContextMenuItem("REMOVE", "Remove")]
        [ContextMenuItem("Stairs", "Stairs")]
        [ContextMenuItem("Chair", "Chair")]
        [ContextMenuItem("Table", "Table")]
        [ContextMenuItem("TorusKnot", "TorusKnot")]
        [ContextMenuItem("Torus", "Torus")]
        [ContextMenuItem("Cone", "Cone")]
        [ContextMenuItem("Cylinder", "Cylinder")]
        [ContextMenuItem("IcoSphere", "IcoSphere")]
        [ContextMenuItem("Sphere", "Sphere")]
        [ContextMenuItem("Wedge", "Wedge")]
        [ContextMenuItem("Box", "Box")]
        [Header("Right Click below to change Primitive")]
        public string currentPrimitiveScript = "";

        [Header("Generate")]
        [Tooltip("Dynamically builds geometry as you change the transform")]
        public bool LiveBuild = true;
        public bool BuildGeomtery = false;
        public bool MeshCollider = true;

        public GameObject selectedOBJ;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update called from PrimitiveGeneratorEditor script
        public void manualUpdate()
        {

            // Which generator script is currently attached

            if (selectedOBJ.GetComponent<BoxGenerator>())
            {
                currentPrimitiveScript = "BoxGenerator";
            }
            else if (selectedOBJ.GetComponent<CylinderGenerator>())
            {
                currentPrimitiveScript = "CylinderGenerator";
            }
            else if (selectedOBJ.GetComponent<SphereGenerator>())
            {
                currentPrimitiveScript = "SphereGenerator";
            }
            else if (selectedOBJ.GetComponent<IcoSphereGenerator>())
            {
                currentPrimitiveScript = "IcoSphereGenerator";
            }
            else if (selectedOBJ.GetComponent<TorusGenerator>())
            {
                currentPrimitiveScript = "TorusGenerator";
            }
            else if (selectedOBJ.GetComponent<TorusKnotGenerator>())
            {
                currentPrimitiveScript = "TorusKnotGenerator";
            }
            else if (selectedOBJ.GetComponent<ConeGenerator>())
            {
                currentPrimitiveScript = "ConeGenerator";
            }
            else if (selectedOBJ.GetComponent<TableGenerator>())
            {
                currentPrimitiveScript = "TableGenerator";
            }
            else if (selectedOBJ.GetComponent<ChairGenerator>())
            {
                currentPrimitiveScript = "ChairGenerator";
            }
            else if (selectedOBJ.GetComponent<StairsGenerator>())
            {
                currentPrimitiveScript = "StairsGenerator";
            }
            else if (selectedOBJ.GetComponent<WedgeGenerator>())
            {
                currentPrimitiveScript = "WedgeGenerator";
            }
            else
            {
                currentPrimitiveScript = "";
            }

            // Generate Mesh

            if (LiveBuild == true || BuildGeomtery == true)
            {
                BuildGeomtery = false;

                // IcoSphere live editing at the level of 3 slows unity significantly, Turning off Live Edit
                if (selectedOBJ.GetComponent<IcoSphereGenerator>() && selectedOBJ.GetComponent<IcoSphereGenerator>().details > 2)
                {
                    LiveBuild = false;
                }


                if (selectedOBJ.GetComponent<BoxGenerator>())
                {
                    selectedOBJ.GetComponent<BoxGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<CylinderGenerator>())
                {
                    selectedOBJ.GetComponent<CylinderGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<SphereGenerator>())
                {
                    selectedOBJ.GetComponent<SphereGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<IcoSphereGenerator>())
                {
                    selectedOBJ.GetComponent<IcoSphereGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<TorusGenerator>())
                {
                    selectedOBJ.GetComponent<TorusGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<TorusKnotGenerator>())
                {
                    selectedOBJ.GetComponent<TorusKnotGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<ConeGenerator>())
                {
                    selectedOBJ.GetComponent<ConeGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<TableGenerator>())
                {
                    selectedOBJ.GetComponent<TableGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<ChairGenerator>())
                {
                    selectedOBJ.GetComponent<ChairGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<StairsGenerator>())
                {
                    selectedOBJ.GetComponent<StairsGenerator>().Generate();
                }
                else if (selectedOBJ.GetComponent<WedgeGenerator>())
                {
                    selectedOBJ.GetComponent<WedgeGenerator>().Generate();
                }
                else
                {
                    currentPrimitiveScript = "";
                }

                if (MeshCollider == true)
                {
                    if (!selectedOBJ.GetComponent<MeshCollider>())
                    {
                        selectedOBJ.AddComponent<MeshCollider>();
                    }
                    selectedOBJ.GetComponent<MeshCollider>().sharedMesh = selectedOBJ.GetComponent<MeshFilter>().sharedMesh;
                }
            }
        }

        // Add and Remove primitive generator scripts

        public void Remove()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
        }
        public void Sphere()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<SphereGenerator>();
        }

        public void Box()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<BoxGenerator>();
        }

        public void IcoSphere()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<IcoSphereGenerator>();
        }

        public void Torus()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<TorusGenerator>();
        }

        public void TorusKnot()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<TorusKnotGenerator>();
        }

        public void Cylinder()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<CylinderGenerator>();
        }

        public void Cone()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<ConeGenerator>();
        }

        public void Table()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<TableGenerator>();
        }

        public void Chair()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<ChairGenerator>();
        }

        public void Stairs()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<StairsGenerator>();
        }

        public void Wedge()
        {
            if (selectedOBJ.GetComponent(currentPrimitiveScript))
            {
                DestroyImmediate(selectedOBJ.GetComponent(currentPrimitiveScript));
            }
            selectedOBJ.AddComponent<WedgeGenerator>();
        }
#endif
    }
}