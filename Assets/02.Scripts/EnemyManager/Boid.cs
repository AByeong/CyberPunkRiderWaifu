// 예시 C# 스크립트 (BoidsController.cs)
using UnityEngine;
using System.Collections.Generic;

public class Boid
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    // Boid 알고리즘에 필요한 기타 데이터 (예: 가속도)
    public Vector3 acceleration;
    public float maxSpeed = 5f;
    public float maxForce = 0.5f;
    public float perceptionRadius = 2.5f; // 이웃을 감지하는 반경
    public float separationRadius = 1.0f; // 분리를 위한 반경

    // 실제 Boid 로직은 여기에 구현됩니다.
    // (이웃 Boid들을 찾고, 분리, 정렬, 응집 규칙을 적용)
    public void flock(List<Boid> allBoids)
    {
        acceleration = Vector3.zero;
        Vector3 separation = Steer(CalculateSeparation(allBoids));
        Vector3 alignment = Steer(CalculateAlignment(allBoids));
        Vector3 cohesion = Steer(CalculateCohesion(allBoids));

        // 가중치 적용 (예시)
        acceleration += separation * 1.5f;
        acceleration += alignment * 1.0f;
        acceleration += cohesion * 1.0f;
    }

    public void UpdatePhysics()
    {
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            rotation = Quaternion.LookRotation(velocity);
        }
    }

    // Steer = desired - velocity (조향력 계산)
    private Vector3 Steer(Vector3 desired)
    {
        Vector3 steer = desired - velocity;
        steer = Vector3.ClampMagnitude(steer, maxForce);
        return steer;
    }

    // --- Boid 규칙 계산 함수들 ---
    private Vector3 CalculateSeparation(List<Boid> allBoids)
    {
        Vector3 steer = Vector3.zero;
        int count = 0;
        foreach (Boid other in allBoids)
        {
            if (other == this) continue;
            float d = Vector3.Distance(position, other.position);
            if (d > 0 && d < separationRadius)
            {
                Vector3 diff = position - other.position;
                diff.Normalize();
                diff /= d; // 거리에 반비례하여 가중치
                steer += diff;
                count++;
            }
        }
        if (count > 0)
        {
            steer /= count;
        }
        if (steer.magnitude > 0)
        {
            steer.Normalize();
            steer *= maxSpeed;
        }
        return steer;
    }

    private Vector3 CalculateAlignment(List<Boid> allBoids)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (Boid other in allBoids)
        {
            if (other == this) continue;
            float d = Vector3.Distance(position, other.position);
            if (d > 0 && d < perceptionRadius)
            {
                sum += other.velocity;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= maxSpeed;
            return sum;
        }
        return Vector3.zero;
    }

    private Vector3 CalculateCohesion(List<Boid> allBoids)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (Boid other in allBoids)
        {
            if (other == this) continue;
            float d = Vector3.Distance(position, other.position);
            if (d > 0 && d < perceptionRadius)
            {
                sum += other.position;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            Vector3 desired = sum - position;
            desired.Normalize();
            desired *= maxSpeed;
            return desired;
        }
        return Vector3.zero;
    }
}


