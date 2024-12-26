using Bb.ComponentModel.Accessors;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bb.Commands
{


    /// <summary>
    /// Represents a context for refreshing objects.
    /// </summary>
    public partial class RefreshContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshContext"/> class.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="transaction"></param>
        public RefreshContext(string sessionId, Transaction transaction)
        {
            this.SessionId = sessionId;
            this.Transaction = transaction;
            this._dicMapper = new Dictionary<Type, IRestorableMapper>();
            this._dicActionAdd = new Dictionary<Type, _Update>();
            this._dicActionRemove = new Dictionary<Type, _Update>();
            this._dicActionUpdate = new Dictionary<Type, _Update>();
        }

        /// <summary>
        /// Adds a model to the context.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        /// <param name="change"></param>
        public void Add(string key, object model, RefreshStrategy change)
        {

            //if (!this._dic.TryGetValue(key, out var c))
            //    this._dic.Add(key, c = new RefreshContextItem(key, model));
            //else
            //    c.Model = model;

            //c.Strategy(change);

        }


        /// <summary>
        /// Gets the session id of command manager
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// Gets the transaction to restore associated with the context.
        /// </summary>
        public Transaction Transaction { get; }


        #region Updates

        public void Apply<T>(RefreshStrategy strategy, T model, string label = null)
        {

            Type type = typeof(T);

            if (string.IsNullOrEmpty(label))
                label = $" {typeof(T)} has been {strategy}";
            _Update? c1;

            switch (strategy)
            {
                case RefreshStrategy.Removed:
                    if (_dicActionRemove.TryGetValue(type, out c1))
                        c1.Execute(model);
                    break;

                case RefreshStrategy.Added:
                    if (_dicActionAdd.TryGetValue(type, out c1))
                        c1.Execute(model);
                    break;

                default:
                case RefreshStrategy.Updated:
                    if (_dicActionUpdate.TryGetValue(type, out c1))
                        c1.Execute(model);
                    break;

            }

            this.Add(label, model, strategy);

        }

        public void Clean()
        {
            _dicActionRemove.Clear();
            _dicActionAdd.Clear();
            _dicActionUpdate.Clear();
        }

        public void Clean<T>()
        {

            if (_dicActionRemove.ContainsKey(typeof(T)))
                _dicActionRemove.Remove(typeof(T));

            if (_dicActionAdd.ContainsKey(typeof(T)))
                _dicActionAdd.Remove(typeof(T));

            if (_dicActionUpdate.ContainsKey(typeof(T)))
                _dicActionUpdate.Remove(typeof(T));

        }

        public void ApplyAfterUpdate<T>(RefreshStrategy strategy, Action<T> action)
        {

            Type type = typeof(T);
            _Update? c1;

            switch (strategy)
            {
                case RefreshStrategy.Removed:
                    if (!_dicActionRemove.TryGetValue(type, out c1))
                        _dicActionRemove.Add(typeof(T), new _Update<T>(action, this));
                    else
                        c1.Append(action);
                    break;

                case RefreshStrategy.Added:
                    if (!_dicActionAdd.TryGetValue(type, out c1))
                        _dicActionAdd.Add(typeof(T), new _Update<T>(action, this));
                    else
                        c1.Append(action);
                    break;

                default:
                case RefreshStrategy.Updated:
                    if (!_dicActionUpdate.TryGetValue(type, out c1))
                        _dicActionUpdate.Add(typeof(T), new _Update<T>(action, this));
                    else
                        c1.Append(action);
                    break;

            }

        }

        public void Register(RefreshStrategy strategy, Action model)
        {

            switch (strategy)
            {
                case RefreshStrategy.Removed:
                    _toDel.Enqueue(model);
                    break;
                case RefreshStrategy.Added:
                    _toAdd.Enqueue(model);
                    break;
                case RefreshStrategy.Updated:
                    _toChange.Enqueue(model);
                    break;
                default:
                    break;
            }

        }

        public bool Remove()
        {
            var result = _toDel.Count > 0;
            while (_toDel.Count > 0)
                _toDel.Dequeue().Invoke();
            return result;
        }

        public bool Add()
        {
            var result = _toAdd.Count > 0;
            while (_toAdd.Count > 0)
                _toAdd.Dequeue().Invoke();
            return result;
        }

        public bool Update()
        {
            var result = _toChange.Count > 0;
            while (_toChange.Count > 0)
                _toChange.Dequeue().Invoke();
            return result;
        }

        #endregion Updates


        private abstract class _Update
        {

            public _Update(RefreshContext parent)
            {
                this._parent = parent;
            }

            protected readonly RefreshContext _parent;

            public abstract void Execute(object model);

            public abstract void Append(object model);

        }

        private class _Update<T> : _Update
        {

            public _Update(Action<T> action, RefreshContext parent)
                : base(parent)
            {
                this._actions = new List<Action<T>>() { action };
            }

            public override void Execute(object model)
            {
                var m = (T)model;
                foreach (Action<T> action in _actions)
                    action(m);
            }

            public override void Append(object model)
            {
                var a = model as Action<T>;
                _actions.Add(a);
            }

            private List<Action<T>> _actions;

        }

        private Dictionary<Type, _Update> _dicActionRemove;
        private Dictionary<Type, _Update> _dicActionAdd;
        private Dictionary<Type, _Update> _dicActionUpdate;

        private Queue<Action> _toAdd = new Queue<Action>();
        private Queue<Action> _toDel = new Queue<Action>();
        private Queue<Action> _toChange = new Queue<Action>();


    }


}
