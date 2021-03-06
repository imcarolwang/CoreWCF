using CoreWCF.Channels;
using System;
using System.ComponentModel;
using System.Security.Authentication.ExtendedProtection;

namespace CoreWCF.Channels
{
    public partial class TcpTransportBindingElement : ConnectionOrientedTransportBindingElement
    {
        int _listenBacklog;
        ExtendedProtectionPolicy _extendedProtectionPolicy;
        TcpConnectionPoolSettings _connectionPoolSettings;

        public TcpTransportBindingElement() : base()
        {
            _listenBacklog = TcpTransportDefaults.GetListenBacklog();
            _connectionPoolSettings = new TcpConnectionPoolSettings();
            _extendedProtectionPolicy = ChannelBindingUtility.DefaultPolicy;
        }
        protected TcpTransportBindingElement(TcpTransportBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
            _listenBacklog = elementToBeCloned._listenBacklog;
            _connectionPoolSettings = elementToBeCloned._connectionPoolSettings.Clone();
            _extendedProtectionPolicy = elementToBeCloned.ExtendedProtectionPolicy;
        }

        public TcpConnectionPoolSettings ConnectionPoolSettings
        {
            get { return _connectionPoolSettings; }
        }

        public int ListenBacklog
        {
            get
            {
                return _listenBacklog;
            }

            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value",
                        SR.ValueMustBePositive));
                }

                _listenBacklog = value;
            }
        }

        public override string Scheme
        {
            get { return "net.tcp"; }
        }

        public ExtendedProtectionPolicy ExtendedProtectionPolicy
        {
            get
            {
                return _extendedProtectionPolicy;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (value.PolicyEnforcement == PolicyEnforcement.Always &&
                    !ExtendedProtectionPolicy.OSSupportsExtendedProtection)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new PlatformNotSupportedException(SR.ExtendedProtectionNotSupported));
                }

                _extendedProtectionPolicy = value;
            }
        }

        public override BindingElement Clone()
        {
            return new TcpTransportBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            // TODO: Decide whether to support DeliveryRequirementsAttribute
            //if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            //{
            //    return (T)(object)new BindingDeliveryCapabilitiesHelper();
            //}
            else if (typeof(T) == typeof(ExtendedProtectionPolicy))
            {
                return (T)(object)ExtendedProtectionPolicy;
            }
            // TODO: Support ITransportCompressionSupport
            //else if (typeof(T) == typeof(ITransportCompressionSupport))
            //{
            //    return (T)(object)new TransportCompressionSupportHelper();
            //}
            else if (typeof(T) == typeof(IConnectionReuseHandler))
            {
                return (T)(object)new ConnectionReuseHandler(new TcpTransportBindingElement(this));
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }
    }
}