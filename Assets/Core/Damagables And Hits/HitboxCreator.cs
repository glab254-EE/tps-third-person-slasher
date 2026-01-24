using UnityEngine;

public static class StatcHitboxCreator 
{
    public static bool TryHitWithBoxHitbox(Vector3 position, Vector3 boxSize,LayerMask mask, double damage, bool OneEnemy = false, Quaternion rotation = new())
    {
        return HandleHits(Physics.OverlapBox(position,boxSize/2,rotation,mask),damage,OneEnemy);
    }
    public static bool TryHitWithRay(Ray ray,float maxdistance,LayerMask mask, double damage, bool OneEnemy = false)
    {
        return HandleHits(Physics.RaycastAll(ray,maxdistance,mask),damage,OneEnemy);
    }
    private static bool HandleHits(object hits, double damage, bool OneEnemy)
    {
        bool found = true;
        
        if (hits.GetType() == typeof(RaycastHit[]))
        {
            RaycastHit[] _hits = (RaycastHit[]) hits;
            
            foreach(RaycastHit collider in _hits)
            {
                if (collider.collider.gameObject.TryGetComponent(out HurtBox box))
                {

                    box.OnHit(damage);

                    if (OneEnemy) return true;

                    found = true;
                }
            }
        } 
        else if (hits.GetType() == typeof(Collider[]))
        {
            Collider[] _hits = (Collider[]) hits;

            foreach(Collider collider in _hits)
            {
                if (collider.gameObject.TryGetComponent(out HurtBox box))
                {

                    box.OnHit(damage);

                    if (OneEnemy) return true;

                    found = true;
                }
            }            
        }
        return found;
    }
}
