﻿using System;

namespace CallMeMaybe.V1 {
    public struct Maybe<T> {
        /*
            Как ты думаешь, почему Maybe - структура?
        */

        /// <summary>
        /// Зачем может быть нужно такое выделенное значение?
        /// Сколько по факту будет экземпляров данного объекта?
        /// </summary>
        // Хочется иметь какую-то константу наподобие null, но просто new Maybe<T>() не подойдет,
        // потому что он не контстантный, и в него можно что-то положить.
        // Экземпляр один, потому что static.
        public static readonly Maybe<T> Nothing = new Maybe<T>();

        public bool HasValue { get; }

        private readonly T _value;

        public T Value =>
            HasValue ? _value : throw new InvalidOperationException($"{typeof(Maybe<T>)} doesn't have value.");

        /// <summary>
        /// Как думаешь, почему я скрыл конструктор?
        /// </summary>
        // Не придумал ситуации, где он мог бы понадобиться. Явно писать "var maybeValue = Maybe<T>(value)" не имеет
        // смысла, так как почему бы напрямую не использоваться value? А если нужен именно Maybe, то короче будет
        // написать неявный каст.
        private Maybe(T value) {
            _value = value;
            HasValue = true;
        }

        /// <inheritdoc />
        public override string ToString() {
            return HasValue ? _value.ToString() : "null";
        }

        public static implicit operator Maybe<T>(T value) {
            /*
                По смыслу это фабрика объектов данного типа (ну или по модному монадный конструктор).
                Т.к. это оператор неявного приведения, позволяет не засорять код кастами.
            */
            return value != null ? new Maybe<T>(value) : Nothing;
        }

        #region Optional useful methods

        public static explicit operator T(Maybe<T> maybe) {
            if (!maybe.HasValue) {
                throw new NullReferenceException();
            }
            return maybe._value;
        }

        public T GetValueOrDefault() => HasValue ? _value : default(T);
        public T GetValueOrDefault(T defaultValue) => HasValue ? _value : defaultValue;

        public Maybe<TResult> Select<TResult>(Func<T, TResult> map) {
            return HasValue ? map(_value) : Maybe<TResult>.Nothing;
        }

        public Maybe<TResult> Select<TResult>(Func<T, Maybe<TResult>> maybeMap) {
            return maybeMap(_value);
        }

        public TResult SelectOrElse<TResult>(Func<T, TResult> map, Func<TResult> elseMap) {
            return HasValue ? map(_value) : elseMap();
        }

        public void Do(Action<T> doAction) {
            if (HasValue) {
                doAction(_value);
            }
        }

        public void DoOrElse(Action<T> doAction, Action elseAction) {
            if (HasValue) {
                doAction(_value);
            } else {
                elseAction();
            }
        }

        public T OrElse(Func<T> elseMap) {
            return HasValue ? _value : elseMap();
        }

        public void OrElseDo(Action elseAction) {
            if (!HasValue) {
                elseAction();
            }
        }

        #endregion
    }
}
