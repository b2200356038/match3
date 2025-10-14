namespace Game.Core
{
    public abstract class ObstacleCell : Cell
    {
        public int Health { get; protected set; }
        protected virtual void OnDestroyed()
        {
            Destroy(gameObject);
        }
        public virtual void TakeDamage(int damage = 1)
        {
            Health -= damage;
            
            if (Health <= 0)
            {
                OnDestroyed();
            }
            else
            {
                UpdateVisual();
            }
        }
        protected virtual void UpdateVisual()
        {
        }
    }
}