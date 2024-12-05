﻿using Bb.Diagnostics;
using System.Diagnostics;

namespace Bb.Commands
{

    /// <summary>
    /// Represents a command transaction that can be committed or rolled back.
    /// </summary>
    public class Transaction : IDisposable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="manager">The manager responsible for handling the transaction.</param>
        /// <param name="name">The name of the transaction.</param>
        /// <param name="index">The index of the transaction.</param>
        internal Transaction(ITransactionManager manager, string name, int index, bool autocommit)
        {

            _activity = ActivityProvider.CreateActivity($"Transaction[{name}]", ActivityKind.Internal);
            if (_activity != null)
            {
                _activity.AddTag("Transaction", name);
                _activity.AddBaggage("Transaction", name);
                _activity.AddBaggage("index", index.ToString());
                _activity.AddBaggage("autocommit", autocommit ? "true" : "false");
                _activity.Start();
            }

            _manager = manager;
            if (manager != null)
            {
                LastStatus = _manager.Status;
                switch (manager.Status)
                {

                    case StatusTransaction.Restoring:
                        Status = CommandTransactionStatus.Restoring;
                        break;

                    case StatusTransaction.InPause:
                        Status = CommandTransactionStatus.Paused;
                        break;

                    case StatusTransaction.Recording:
                    case StatusTransaction.Waiting:
                    default:
                        Status = CommandTransactionStatus.Recording;
                        break;
                }
                this.Count = manager.Count;
            }

            Index = index;
            Label = name;
            this._autocommit = autocommit;

        }

        /// <summary>
        /// Gets or sets the label of the transaction.
        /// </summary>
        public string Label { get; private set; }


        public int Count { get; }

        /// <summary>
        /// Gets the index of the transaction.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the stream associated with the transaction.
        /// </summary>
        public Stream StreamReader
        {
            get
            {
                return _manager.GetStreamForReading(this);
            }
        }


        public CommandTransactionStatus Status { get; internal set; }

        public StatusTransaction LastStatus { get; set; }

        public bool ResumeToEnd { get; internal set; }




        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit(string? label = null)
        {
            _commited = true;
            if (!string.IsNullOrEmpty(label))
                Label = label;
        }

        /// <summary>
        /// Disposes the transaction and rolls back any changes.
        /// </summary>
        public void Dispose()
        {

            try
            {

                if (this._commited || _autocommit)
                    _manager.Commit();

                else
                    _manager.Rollback();

            }
            finally
            {
                _activity?.Stop();
                _activity?.Dispose();
            }

        }

        /// <summary>
        /// Gets a view of the transaction.
        /// </summary>
        /// <returns>A <see cref="TransactionView"/> representing the transaction view.</returns>
        public TransactionView GetView()
        {
            return new TransactionView(this);
        }


        /// <summary>
        /// Initializes the value for the transaction.
        /// </summary>
        /// <param name="target">The target command memorizer.</param>
        public void Save(IMemorizer target)
        {

            using (var stream = _manager.GetStreamForWriting(this))
            {
                target.Memorize(stream);
                stream.Flush();
                _saved = true;
            }

            Status = CommandTransactionStatus.Registered;

        }

        public uint? GetCrc()
        {

            if (!_crc32.HasValue && _saved)
                using (var stream = _manager.GetStreamForReading(this))
                    _crc32 = stream.ToBytes().CalculateCrc32();

            return _crc32;

        }

        private uint? _crc32;
        private bool _autocommit;
        private bool _saved;
        private string _path;
        private bool _commited;
        private readonly Activity? _activity;
        private readonly ITransactionManager _manager;

        //private readonly int _crc;
    }

    public enum CommandTransactionStatus
    {
        Recording,
        Committed,
        Registered,
        Paused,
        Restoring
    }

}
