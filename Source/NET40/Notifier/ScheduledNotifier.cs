﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Diagnostics.Contracts;

namespace Codeplex.Reactive.Notifier
{
    public class ScheduledNotifier<T> : IObservable<T>, IProgress<T>
    {
        readonly IScheduler scheduler;
        readonly Subject<T> trigger = new Subject<T>();

        public ScheduledNotifier()
        {
            this.scheduler = Scheduler.Immediate;
        }

        public ScheduledNotifier(IScheduler scheduler)
        {
            Contract.Requires<ArgumentNullException>(scheduler != null);

            this.scheduler = scheduler;
        }

        public void Report(T value)
        {
            scheduler.Schedule(() => trigger.OnNext(value));
        }

        public IDisposable Report(T value, TimeSpan dueTime)
        {
            Contract.Ensures(Contract.Result<IDisposable>() != null);

            var cancel = scheduler.Schedule(dueTime, () => trigger.OnNext(value));

            Contract.Assume(cancel != null);
            return cancel;
        }

        public IDisposable Report(T value, DateTimeOffset dueTime)
        {
            Contract.Ensures(Contract.Result<IDisposable>() != null);

            var cancel = scheduler.Schedule(dueTime, () => trigger.OnNext(value));

            Contract.Assume(cancel != null);
            return cancel;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return trigger.Subscribe(observer);
        }
    }
}