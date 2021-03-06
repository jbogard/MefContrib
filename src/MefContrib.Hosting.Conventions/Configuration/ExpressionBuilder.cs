namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /*
     *  This class needs refactoring before going live 
     */

    public interface IExpressionBuilder
    {
        object Build();
    }

    public abstract class ExpressionBuilder<T> : IExpressionBuilder where T : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionBuilder{T}"/> class.
        /// </summary>
        protected ExpressionBuilder()
        {
            this.MemberValues = new Dictionary<MemberInfo, List<object>>();
        }

        protected IDictionary<MemberInfo, List<object>> MemberValues { get; set; }

        public object Build()
        {
            var instance = new T();

            foreach (var member in this.MemberValues.Keys)
            {
                this.SetMemberValue(member, instance);
            }

            return instance;
        }

        public void ProvideValueFor(Expression<Func<T, object>> expression, object value)
        {
            var member =
                expression.GetTargetMemberInfo();

            if (member == null)
            {
                throw new InvalidOperationException();
            }

            if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
            {
                throw new InvalidOperationException();
            }

            this.StoreMemberValue(member, value);
        }

        private static void SetCollectionValue(MemberInfo member, object instance, List<object> value)
        {
            var bindingFlags =
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.InvokeMethod;

            object collection = null;

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    collection = ((FieldInfo)member).GetValue(instance);
                    break;

                case MemberTypes.Property:
                    collection = ((PropertyInfo)member).GetValue(instance, null);
                    break;
            }

            var method = collection
                .GetType()
                .GetMethod("Add", bindingFlags);

            value.ForEach(x => method.Invoke(collection, bindingFlags, null, new[] { x }, null));
        }

        private void SetMemberValue(MemberInfo member, object instance)
        {
            var value =
                this.MemberValues[member];

            var memberType =
                member.GetContractMember();

            if (memberType.IsCollection())
            {
                SetCollectionValue(member, instance, value);
            }
            else
            {
                SetSingleValue(member, instance, value);
            }
        }

        private static void SetSingleValue(MemberInfo member, object instance, IEnumerable<object> value)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)member).SetValue(instance, value.First());
                    break;

                case MemberTypes.Property:
                    ((PropertyInfo)member).SetValue(instance, value.First(), null);
                    break;
            }
        }

        private void StoreMemberValue(MemberInfo member, object value)
        {
            if (!this.MemberValues.ContainsKey(member))
            {
                this.MemberValues.Add(member, new List<object>());
            }

            var valueList =
                this.MemberValues[member];

            if (value.GetType().IsEnumerable())
            {
                valueList.AddRange(((IEnumerable)value).Cast<object>());
            }
            else
            {
                valueList.Add(value);
            }
        }
    }
}