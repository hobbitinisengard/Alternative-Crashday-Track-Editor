﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UndoBuffer : MonoBehaviour
{

  /// <summary>
  /// Global coordinates of former vertices before last terrain operation
  /// </summary>
  private static List<Vector3> UndoZnaczniki = new List<Vector3>();
  /// <summary>
  /// Clear buffer before next call of AddZnacznik
  /// </summary>
  private static bool Clear_buffer_before_next_add = false;

  public static void AddZnacznik(int x, int z)
  {
    Vector3 mrk = new Vector3(x, 0, z);
    if (Clear_buffer_before_next_add)
    {
      UndoZnaczniki.Clear();
      Clear_buffer_before_next_add = false;
    }
    UndoZnaczniki.Add(mrk);
  }
  /// <summary>
  /// Adds new znacznik to buffer. Moreover if ApplyOperation was run before, f. will clear buffer once before addition.
  /// </summary>
  /// <param name="mrk"></param>
  public static void AddZnacznik(Vector3 mrk)
  {
    if (Clear_buffer_before_next_add)
    {
      UndoZnaczniki.Clear();
      Clear_buffer_before_next_add = false;
    }
    UndoZnaczniki.Add(mrk);
  }
  /// <summary>
  /// Saves list of znaczniki to buffer as one operation to possible undo
  /// </summary>
  /// <param name="Mrks"></param>
  public static void AddOperation(List<Vector3> Mrks)
  {
    UndoZnaczniki = Mrks.ToList();
  }
  /// <summary>
  /// Signalizes that next call of AddZnacznik will belong to new operation
  /// </summary>
  public static void ApplyOperation()
  {
    Clear_buffer_before_next_add = true;
    UndoZnaczniki = UndoZnaczniki.Distinct().ToList();
  }
  /// <summary>
  /// Recovers all height points that were previously added to buffer as undo
  /// </summary>
  public static void PasteUndoZnaczniki()
  {
    //Indexes of vertices for UpdateMapColliders()
    List<int> indexes = new List<int>();

    // List of tiles lying onto vertices that are now being pasted
    List<GameObject> to_update = new List<GameObject>();
    foreach (var mrk in UndoZnaczniki)
    {
      if (Service.IsWithinMapBounds(mrk))
      {
        // Update arrays of vertex heights
        indexes.Add(Service.PosToIndex(mrk));
        Service.current_heights[indexes[indexes.Count - 1]] = mrk.y;
        Service.former_heights[indexes[indexes.Count - 1]] = mrk.y;

        Vector3 pom = mrk;

        // Mark pasted vertices
        GameObject zn = Service.MarkAndReturnZnacznik(pom);
        if (zn != null)
          zn.transform.position = new Vector3(zn.transform.position.x, mrk.y, zn.transform.position.z);

        // Look for tiles lying here
        {
          RaycastHit tile;
          pom.y = Service.maxHeight;
          if (Physics.SphereCast(pom, 0.1f, Vector3.down, out tile, Service.maxHeight - Service.minHeight, 1 << 9) && !to_update.Contains(tile.transform.gameObject))
            to_update.Add(tile.transform.gameObject);
        }
      }
    }
    Service.UpdateMapColliders(indexes);
    Build.UpdateTiles(to_update);
    UndoZnaczniki.Clear();
  }
  
}

