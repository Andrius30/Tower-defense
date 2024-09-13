using System;
using System.Collections.Generic;
using UnityEngine;

public class Collision2DListener : MonoBehaviour
{
    private class CollisionData
    {
        public Collider2D other = null;
        public float addedTimestamp = 0;

        public bool hasContainTriggered = false;
    }

    //[Flags]
    //public enum CollisionTags
    //{
    //    Undef = (1 << 0),
    //    Player = (1 << 1),
    //    Enemy = (1 << 2),
    //    Boss = (1 << 3),
    //    Collectable = (1 << 4)
    //}

    //public const CollisionTags ALL_TAGS = CollisionTags.Undef | CollisionTags.Player | CollisionTags.Enemy | CollisionTags.Boss | CollisionTags.Collectable;

    //[SerializeField]
    //[EnumFlagsAttribute]
    //private CollisionTags collisionTags = ALL_TAGS;

    [SerializeField]
    private List<EventDelegate> triggerEnter2DDelegates;
    public List<EventDelegate> TriggerEnter2DDelegates
    {
        get { return (triggerEnter2DDelegates ?? (triggerEnter2DDelegates = new List<EventDelegate>())); }
    }
    [SerializeField]
    private List<EventDelegate> triggerContain2DDelegates;
    public List<EventDelegate> TriggerContain2DDelegates
    {
        get { return (triggerContain2DDelegates ?? (triggerContain2DDelegates = new List<EventDelegate>())); }
    }
    [SerializeField]
    private List<EventDelegate> triggerExit2DDelegates;
    public List<EventDelegate> TriggerExit2DDelegates
    {
        get { return (triggerExit2DDelegates ?? (triggerExit2DDelegates = new List<EventDelegate>())); }
    }

    public Collider2D refCollider2D; 

    public bool disableOnTrigger = false;
    public float containDuration = 0f;

    public event Action<Collider2D> TriggerEnter2DCallback;
    public event Action<Collider2D> TriggerContain2DCallback;
    public event Action<Collider2D> TriggerExit2DCallback;

    private Dictionary<Collider2D, CollisionData> collisions = new Dictionary<Collider2D, CollisionData>();

    void Update()
    {
        if (containDuration > 0)
        {
            foreach (var entry in collisions)
            {
                if (entry.Value.hasContainTriggered)
                    continue;

                if (Time.time - entry.Value.addedTimestamp >= containDuration)
                {
                    EventDelegate.Execute(TriggerContain2DDelegates, new EventDelegate.Parameter[1] { new EventDelegate.Parameter(entry.Value.other) });

                    if (TriggerContain2DCallback != null)
                    {
                        TriggerContain2DCallback(entry.Value.other);
                    }

                    entry.Value.hasContainTriggered = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //CollisionTags tag = (CollisionTags)CollisionTags.Undef.Get(other.tag);
        //if (tag == 0)
        //    tag = CollisionTags.Undef;

        //if (collisionTags.ContainsFlag(tag))
        {
            EventDelegate.Execute(TriggerEnter2DDelegates, new EventDelegate.Parameter[1] { new EventDelegate.Parameter(other) });

            if (TriggerEnter2DCallback != null)
            {
                TriggerEnter2DCallback(other);
            }

            if (containDuration > 0)
            {
                CollisionData data = null;
                if (!collisions.ContainsKey(other))
                {
                    data = new CollisionData();
                    data.other = other;

                    collisions.Add(other, data);
                }
                else
                {
                    data = collisions[other];
                }

                data.addedTimestamp = Time.time;
            }

            if (TriggerEnter2DDelegates.Count > 0 || TriggerEnter2DCallback != null)
            {
                if (disableOnTrigger)
                {
                    GetComponent<Collider2D>().enabled = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //CollisionTags tag = (CollisionTags)CollisionTags.Undef.Get(other.tag);

        //if (collisionTags.ContainsFlag(tag))
        {
            EventDelegate.Execute(TriggerExit2DDelegates, new EventDelegate.Parameter[1] { new EventDelegate.Parameter(other) });

            if (TriggerExit2DCallback != null)
            {
                TriggerExit2DCallback(other);
            }

            if (containDuration > 0)
            {
                if (collisions.ContainsKey(other))
                {
                    collisions.Remove(other);
                }
            }

            if (TriggerExit2DDelegates.Count > 0 || TriggerExit2DCallback != null)
            {
                if (disableOnTrigger)
                {
                    GetComponent<Collider2D>().enabled = false;
                    //this.enabled = false;
                }
            }
        }
    }
}

