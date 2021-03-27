using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UniRx;
using Cysharp.Threading.Tasks;

namespace DemoProj
{
    public class UITweenSequence : MonoBehaviour
    {
        [Serializable]
        public class Param
        {
            public UITweenerBase tweener;
            public float duration;
            public Param(UITweenerBase target, float duration)
            {
                this.tweener = target;
                this.duration = duration;
            }
        }

        [Serializable]
        public class ParamList
        {
            public float delay = 0;
            public List<Param> list = new List<Param>();
        }

        public List<ParamList> playList;
        public List<ParamList> reversePlayList;

        private List<Sequence> sequenceList;

        private Subject<Unit> _observable;

        public bool isPlaying { private set; get; }

        private void Reset()
        {
            playList = new List<ParamList>();
            reversePlayList = new List<ParamList>();
            playList.Add(new ParamList());
            reversePlayList.Add(new ParamList());
            var allTweener = GetComponentsInChildren<UITweenerBase>(true).ToList();

            List<UITweenerBase> subTweeners = new List<UITweenerBase>();
            var subSequences = GetComponentsInChildren<UITweenSequence>(true);
            foreach (var item in subSequences)
            {
                if (item == this) continue;
                foreach (var parts in item.playList)
                {
                    foreach (var param in parts.list)
                    {
                        subTweeners.Add(param.tweener);
                    }
                }
                foreach (var parts in item.reversePlayList)
                {
                    foreach (var param in parts.list)
                    {
                        subTweeners.Add(param.tweener);
                    }
                }
            }
            foreach (var item in allTweener)
            {
                if (!item.isReverse && !subTweeners.Contains(item))
                    playList[0].list.Add(new Param(item, .2f));
                else if (item.isReverse && !subTweeners.Contains(item))
                    reversePlayList[0].list.Add(new Param(item, .2f));
            }
        }

        public void Play(Action onComplete = null, bool isInteractable = false, float delay = 0)
        {
            sequenceList = new List<Sequence>();
            Sequence longest = DOTween.Sequence();
            float maxEndTime = 0;
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            foreach (var paramList in playList)
            {
                float longestDuration = 0;
                var sequence = DOTween.Sequence();
                foreach (var item in paramList.list)
                {
                    if (item.duration > longestDuration)
                    {
                        longestDuration = item.duration;
                    }
                    item.tweener.Init();
                    if (item.tweener.gameObject)
                    {
                        if (!item.tweener.keepAlive && !item.tweener.gameObject.activeSelf)
                            item.tweener.gameObject.SetActive(true);
                    }
                    sequence.Join(item.tweener.Play(item.duration));
                }
                sequence.SetDelay(paramList.delay + delay);
                sequenceList.Add(sequence);
                if (longestDuration + paramList.delay > maxEndTime)
                {
                    longest = sequence;
                    maxEndTime = longestDuration + paramList.delay;
                }
            }
            longest.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
            sequenceList.ForEach(i => i.Play());
        }

        public async UniTask PlayAsync(UnityAction onComplete = null, bool isInteractable = false, float delay = 0)
        {
            isPlaying = true;
            Play(() =>
            {
                isPlaying = false;
            }, isInteractable, delay);
            await new WaitUntil(() => isPlaying == false);
            onComplete?.Invoke();
        }

        public void PlayReverse(Action onComplete = null, bool isInteractable = false, float delay = 0)
        {
            sequenceList = new List<Sequence>();
            Sequence longest = DOTween.Sequence();
            float maxEndTime = 0;
            foreach (var paramList in reversePlayList)
            {
                float longestDuration = 0;
                var sequence = DOTween.Sequence();
                foreach (var item in paramList.list)
                {
                    if (item.duration > longestDuration)
                    {
                        longestDuration = item.duration;
                    }
                    item.tweener.Init();
                    sequence.Join(item.tweener.Play(item.duration).OnComplete(() =>
                    {
                        if (item.tweener.gameObject && !item.tweener.keepAlive)
                        {
                            item.tweener.gameObject.SetActive(false);
                        }
                    }));
                }
                sequence.SetDelay(paramList.delay + delay);
                sequenceList.Add(sequence);
                if (longestDuration + paramList.delay > maxEndTime)
                {
                    longest = sequence;
                    maxEndTime = longestDuration + paramList.delay;
                }
            }
            longest.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
            sequenceList.ForEach(i => i.Play());
        }

        public async UniTask PlayReverseAsync(UnityAction onComplete = null, bool isInteractable = false, float delay = 0)
        {
            isPlaying = true;
            PlayReverse(() =>
            {
                isPlaying = false;
            }, isInteractable, delay);
            await new WaitUntil(() => isPlaying == false);
            onComplete?.Invoke();
        }


        public IObservable<Unit> PlayAsObservable()
        {
            _observable = new Subject<Unit>();
            PlayAsync(() =>
            {
                _observable.OnNext(Unit.Default);
                _observable.OnCompleted();
                _observable.Dispose();
            }).Forget();
            return _observable;
        }

        public IObservable<Unit> PlayReverseAsObservable()
        {
            _observable = new Subject<Unit>();
            PlayReverseAsync(() =>
            {
                _observable.OnNext(Unit.Default);
                _observable.OnCompleted();
                _observable.Dispose();
            }).Forget();
            return _observable;
        }



        public void ResetParts()
        {
            foreach (var item in playList)
            {
                foreach (var i in item.list)
                {
                    i.tweener.Init();
                }
            }
        }

        public void Kill()
        {
            isPlaying = false;
            if (sequenceList != null)
                sequenceList.ForEach(i => i.Kill(true));
        }

        private void OnDestroy()
        {
            if (sequenceList != null)
                sequenceList.ForEach(i => i.Kill(true));
        }
    }
}
