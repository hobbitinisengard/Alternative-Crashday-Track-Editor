﻿using System.IO;
using UnityEngine;
//Obsługa bieżącego zaznaczenia
public class Duint
{
    public int x;
    public int z;
    //dot. obszarów tiles;
    public Duint(int X, int Z)
    {
        x = X;
        z = Z;
    }
}
public class Highlight : MonoBehaviour
{
    /// <summary>
    /// Vertexowy wymiar Lewego Dolnego Rogu elementu
    /// </summary>
	public static Vector3Int t = new Vector3Int();
    public static bool nad = false;
    public static Vector3Int pos; //dot. konkretynych vertexów (podniesiony o 0.01f)
    RaycastHit hit;
    float last_z = 99;

    void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        pos = get_valid_init_vector(r);
        if (pos.x == -1)
        {
            nad = false;
        }
        else
        {
            t.x = 4 * Mathf.FloorToInt(pos.x / 4f); //Zwraca lewy dolny róg bieżącej trawki
            t.z = 4 * Mathf.FloorToInt(pos.z / 4f);
            nad = true;
        }
    }

    //Zwraca wektor pozycji verta. Działa w ramach Mapy
    Vector3Int get_valid_init_vector(Ray r)
    {
        bool traf = Physics.Raycast(r.origin, r.direction, out hit, 300, 1 << 8);
        if (traf && hit.transform.gameObject.layer != 5)
        { // Raycast nie przejdzie przez elementy UI
            return Vector3Int.RoundToInt(hit.point);
        }
        else
        {
            return new Vector3Int(-1, -1, -1);
        }
    }


    void DebugRayCast(Vector3 pos, Ray r)
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (Physics.Raycast(pos, Vector3.down, out hit, Terenowanie.rayHeight, 1 << 12))
            {
                StreamWriter writer = new StreamWriter("Assets/Resources/flatters.txt", true);
                writer.Write(hit.transform.name.Substring(0, hit.transform.name.IndexOf('(')) + " ");
                Debug.Log("name =" + hit.transform.name.Substring(0, hit.transform.name.IndexOf('(')));
                writer.Close();
            }
        }
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            RaycastHit hit1;
            if (Physics.Raycast(r.origin, r.direction, out hit1, Terenowanie.rayHeight, 1 << 12) && Physics.Raycast(new Vector3(Mathf.Round(pos.x), hit1.point.y + 0.5f, Mathf.Round(pos.z)), Vector3.down, out hit, Terenowanie.rayHeight, 1 << 12) && Mathf.Round(hit.point.z) != Mathf.Round(last_z))
            {
                StreamWriter writer = new StreamWriter("Assets/Resources/flatters.txt", true);
                writer.Write(hit.point.y + " ");
                last_z = Mathf.Round(hit.point.z);
                writer.Close();
                Debug.Log("DEBUG CAST Y=" + hit.point + "lastz=" + last_z);

            }

        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Physics.Raycast(r.origin, r.direction, out hit, Terenowanie.rayHeight, 1 << 12))
                Debug.Log("free Y=" + hit.point.y);
        }
    }

}