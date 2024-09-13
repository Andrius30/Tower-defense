using Andrius.Core.Debuging;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Andrius.Core.Utils.Timers
{
    public class SequenceTimer
    {
        class Sequence
        {
            public float time;
            public Action onUpdate;
            public Action onDone;
            public bool isDone;

            public Sequence(float time, Action onUpdate, Action onDone, bool isDone)
            {
                this.time = time;
                this.onUpdate = onUpdate;
                this.onDone = onDone;
                this.isDone = isDone;
            }
            public void OnUpdate()
            {
                if (!isDone)
                {
                    UpdateTime();
                    onUpdate?.Invoke();
                }
            }
            public string GetOnUpdateName() => onUpdate.Method.Name;
            public string GetOnDoneName() => onDone.Method.Name;
            void UpdateTime()
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    isDone = true;
                    time = 0;
                    onDone?.Invoke();
                }
            }
        }

        Action onSequenceDone;
        bool isSequenceDone = true;
        List<Sequence> sequences = new List<Sequence>();
        int currentIndex = 0;

        public SequenceTimer(Action onSequenceDone) => this.onSequenceDone = onSequenceDone;

        public void OnUpdate()
        {
            if (sequences.Count <= 0) return;
            if (isSequenceDone) return;
            if (sequences[currentIndex].isDone)
            {
                currentIndex++;
                if (currentIndex > sequences.Count - 1)
                {
                    isSequenceDone = true;
                    currentIndex = 0;
                    onSequenceDone?.Invoke();
                    sequences.Clear();
                    return;
                }
            }
            sequences[currentIndex].OnUpdate();
        }
        public List<string> GetSequenceFunctionNames()
        {
            List<string> names = new List<string>();
            foreach (var sequence in sequences)
            {
                names.Add(sequence.GetOnUpdateName());
                names.Add(sequence.GetOnDoneName());
            }
            return names;
        }
        public void AddToSequence(float time, Action onUpdate, Action onDone)
        {
            Sequence sequence = new Sequence(time, onUpdate, onDone, false);
            sequences.Add(sequence);
        }
        public void StartSequence() => isSequenceDone = false;
        public bool IsSquenceDone() => isSequenceDone;
    }
}