using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
      public static CameraManager instance;

      [Header("Camera Follow")]
      public float m_TravelTime;
      public Vector2 m_Target;
      private bool m_Finished;
      private bool m_LockCam;

      [Header("Camera Shake")]
      public float m_Duration;
      public float m_Magnitude;
      public float m_Power;
      private Vector3 m_OriginPos;

      void Awake()
      {
          instance = this;
      }

      public void SetCameraTarget(Vector2 pos)
      {
          m_Target = pos;

          if(!m_LockCam)
          {
              StartCoroutine("MoveCamera");
          }
      }

      public void LockCamera(bool state)
      {
          m_LockCam = state;
      }

      public void ShakeCamera(float duration, float magnitude, float power)
      {
          m_Duration = duration;
          m_Magnitude = magnitude;
          m_Power = power;

          LockCamera(true);
          m_OriginPos = this.transform.position;
          InvokeRepeating("CamShaking", 0f, 0.005f);
          Invoke("CamStopShaking", m_Duration);

      }

      void CamShaking()
      {
          float x = Random.Range(-1f, 1f) * (m_Magnitude * m_Power);
          float y = Random.Range(-1f, 1f) * (m_Magnitude * m_Power);
          transform.position = new Vector3(m_OriginPos.x + x, m_OriginPos.y + y, m_OriginPos.z);
      }

      void CamStopShaking()
      {
          CancelInvoke("CamShaking");
          LockCamera(false);
          transform.position = new Vector3(m_Target.x, m_Target.y, transform.position.z);
      }

      public IEnumerator MoveCamera()
      {
          foreach(Enemy enemy in DungeonManager.instance.m_AllEnemies)
          {
              enemy.m_CanMove = false;
          }

          Vector3 newPos = transform.position;
          newPos.x = m_Target.x;
          newPos.y = m_Target.y;

          float t = 0.0f;
          while(t <= 1)
          {
              t += Time.deltaTime / m_TravelTime;
              transform.position = Vector3.Lerp(transform.position, newPos, t);
              yield return null;
          }

          foreach(Enemy enemy in DungeonManager.instance.m_AllEnemies)
          {
              enemy.m_CanMove = true;
          }

          yield return null;
      }
}
