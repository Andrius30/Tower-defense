using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionListener : MonoBehaviour
{
    private class CollisionData
    {
        public Collider other = null;
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
    private List<EventDelegate> triggerEnterDelegates;
    public List<EventDelegate> TriggerEnterDelegates
    {
        get { return (triggerEnterDelegates ?? (triggerEnterDelegates = new List<EventDelegate>())); }
    }
    [SerializeField]
    private List<EventDelegate> triggerContainDelegates;
    public List<EventDelegate> TriggerContainDelegates
    {
        get { return (triggerContainDelegates ?? (triggerContainDelegates = new List<EventDelegate>())); }
    }
    [SerializeField]
    private List<EventDelegate> triggerExitDelegates;
    public List<EventDelegate> TriggerExitDelegates
    {
        get { return (triggerExitDelegates ?? (triggerExitDelegates = new List<EventDelegate>())); }
    }

    public Collider refCollider; 

    public bool disableOnTrigger = false;
    public float containDuration = 0f;

    public event Action<Collider> TriggerEnterCallback;
    public event Action<Collider> TriggerContainCallback;
    public event Action<Collider> TriggerExitCallback;

    private Dictionary<Collider, CollisionData> collisions = new Dictionary<Collider, CollisionData>();

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
                    EventDelegate.Execute(TriggerContainDelegates, new EventDelegate.Parameter[1] { new EventDelegate.Parameter(entry.Value.other) });

                    if (TriggerContainCallback != null)
                    {
                        TriggerContainCallback(entry.Value.other);
                    }

                    entry.Value.hasContainTriggered = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //CollisionTags tag = (CollisionTags)CollisionTags.Undef.Get(other.tag);
        //if (tag == 0)
        //    tag = CollisionTags.Undef;

        //if (collisionTags.ContainsFlag(tag))
        {
            EventDelegate.Execute(TriggerEnterDelegates, new EventDelegate.Parameter[1] { new EventDelegate.Parameter(other) });

            if (TriggerEnterCallback != null)
            {
                TriggerEnterCallback(other);
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

            if (TriggerEnterDelegates.Count > 0 || TriggerEnterCallback != null)
            {
                if (disableOnTrigger)
                {
                    GetComponent<Collider>().enabled = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //CollisionTags tag = (CollisionTags)CollisionTags.Undef.Get(other.tag);

        //if (collisionTags.ContainsFlag(tag))
        {
            EventDelegate.Execute(TriggerExitDelegates, new EventDelegate.Parameter[1] { new EventDelegate.Parameter(other) });

            if (TriggerExitCallback != null)
            {
                TriggerExitCallback(other);
            }

            if (containDuration > 0)
            {
                if (collisions.ContainsKey(other))
                {
                    collisions.Remove(other);
                }
            }

            if (TriggerExitDelegates.Count > 0 || TriggerExitCallback != null)
            {
                if (disableOnTrigger)
                {
                    GetComponent<Collider>().enabled = false;
                    //this.enabled = false;
                }
            }
        }
    }
}

