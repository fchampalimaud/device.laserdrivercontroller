using Bonsai;
using Bonsai.Harp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Harp.LaserDriverController
{
    /// <summary>
    /// Generates events and processes commands for the LaserDriverController device connected
    /// at the specified serial port.
    /// </summary>
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    [Description("Generates events and processes commands for the LaserDriverController device.")]
    public partial class Device : Bonsai.Harp.Device, INamedElement
    {
        /// <summary>
        /// Represents the unique identity class of the <see cref="LaserDriverController"/> device.
        /// This field is constant.
        /// </summary>
        public const int WhoAmI = 1298;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : base(WhoAmI) { }

        string INamedElement.Name => nameof(LaserDriverController);

        /// <summary>
        /// Gets a read-only mapping from address to register type.
        /// </summary>
        public static new IReadOnlyDictionary<int, Type> RegisterMap { get; } = new Dictionary<int, Type>
            (Bonsai.Harp.Device.RegisterMap.ToDictionary(entry => entry.Key, entry => entry.Value))
        {
            { 32, typeof(SpadSwitch) },
            { 33, typeof(LaserState) },
            { 34, typeof(Reserved0) },
            { 35, typeof(Reserved1) },
            { 36, typeof(Reserved2) },
            { 37, typeof(Reserved13) },
            { 38, typeof(LaserFrequencySelect) },
            { 39, typeof(LaserIntensity) },
            { 40, typeof(OutputSet) },
            { 41, typeof(OutputClear) },
            { 42, typeof(OutputToggle) },
            { 43, typeof(OutputState) },
            { 44, typeof(BncsState) },
            { 45, typeof(SignalState) },
            { 46, typeof(Bnc1On) },
            { 47, typeof(Bnc1Off) },
            { 48, typeof(Bnc1Pulses) },
            { 49, typeof(Bnc1Tail) },
            { 50, typeof(Bnc2On) },
            { 51, typeof(Bnc2Off) },
            { 52, typeof(Bnc2Pulses) },
            { 53, typeof(Bnc2Tail) },
            { 54, typeof(SignalAOn) },
            { 55, typeof(SignalAOff) },
            { 56, typeof(SignalAPulses) },
            { 57, typeof(SignalATail) },
            { 58, typeof(SignalBOn) },
            { 59, typeof(SignalBOff) },
            { 60, typeof(SignalBPulses) },
            { 61, typeof(SignalBTail) },
            { 62, typeof(EventEnable) }
        };

        /// <summary>
        /// Gets the contents of the metadata file describing the <see cref="LaserDriverController"/>
        /// device registers.
        /// </summary>
        public static readonly string Metadata = GetDeviceMetadata();

        static string GetDeviceMetadata()
        {
            var deviceType = typeof(Device);
            using var metadataStream = deviceType.Assembly.GetManifestResourceStream($"{deviceType.Namespace}.device.yml");
            using var streamReader = new System.IO.StreamReader(metadataStream);
            return streamReader.ReadToEnd();
        }
    }

    /// <summary>
    /// Represents an operator that returns the contents of the metadata file
    /// describing the <see cref="LaserDriverController"/> device registers.
    /// </summary>
    [Description("Returns the contents of the metadata file describing the LaserDriverController device registers.")]
    public partial class GetDeviceMetadata : Source<string>
    {
        /// <summary>
        /// Returns an observable sequence with the contents of the metadata file
        /// describing the <see cref="LaserDriverController"/> device registers.
        /// </summary>
        /// <returns>
        /// A sequence with a single <see cref="string"/> object representing the
        /// contents of the metadata file.
        /// </returns>
        public override IObservable<string> Generate()
        {
            return Observable.Return(Device.Metadata);
        }
    }

    /// <summary>
    /// Represents an operator that groups the sequence of <see cref="LaserDriverController"/>" messages by register type.
    /// </summary>
    [Description("Groups the sequence of LaserDriverController messages by register type.")]
    public partial class GroupByRegister : Combinator<HarpMessage, IGroupedObservable<Type, HarpMessage>>
    {
        /// <summary>
        /// Groups an observable sequence of <see cref="LaserDriverController"/> messages
        /// by register type.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of observable groups, each of which corresponds to a unique
        /// <see cref="LaserDriverController"/> register.
        /// </returns>
        public override IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(message => Device.RegisterMap[message.Address]);
        }
    }

    /// <summary>
    /// Represents an operator that writes the sequence of <see cref="LaserDriverController"/>" messages
    /// to the standard Harp storage format.
    /// </summary>
    [Description("Writes the sequence of LaserDriverController messages to the standard Harp storage format.")]
    public partial class DeviceDataWriter : Sink<HarpMessage>, INamedElement
    {
        const string BinaryExtension = ".bin";
        const string MetadataFileName = "device.yml";
        readonly Bonsai.Harp.MessageWriter writer = new();

        string INamedElement.Name => nameof(LaserDriverController) + "DataWriter";

        /// <summary>
        /// Gets or sets the relative or absolute path on which to save the message data.
        /// </summary>
        [Description("The relative or absolute path of the directory on which to save the message data.")]
        [Editor("Bonsai.Design.SaveFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string Path
        {
            get => System.IO.Path.GetDirectoryName(writer.FileName);
            set => writer.FileName = System.IO.Path.Combine(value, nameof(LaserDriverController) + BinaryExtension);
        }

        /// <summary>
        /// Gets or sets a value indicating whether element writing should be buffered. If <see langword="true"/>,
        /// the write commands will be queued in memory as fast as possible and will be processed
        /// by the writer in a different thread. Otherwise, writing will be done in the same
        /// thread in which notifications arrive.
        /// </summary>
        [Description("Indicates whether writing should be buffered.")]
        public bool Buffered
        {
            get => writer.Buffered;
            set => writer.Buffered = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to overwrite the output file if it already exists.
        /// </summary>
        [Description("Indicates whether to overwrite the output file if it already exists.")]
        public bool Overwrite
        {
            get => writer.Overwrite;
            set => writer.Overwrite = value;
        }

        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [Description("Specifies how the message filter will use the matching criteria.")]
        public FilterType FilterType
        {
            get => writer.FilterType;
            set => writer.FilterType = value;
        }

        /// <summary>
        /// Gets or sets a value specifying the expected message type. If no value is
        /// specified, all messages will be accepted.
        /// </summary>
        [Description("Specifies the expected message type. If no value is specified, all messages will be accepted.")]
        public MessageType? MessageType
        {
            get => writer.MessageType;
            set => writer.MessageType = value;
        }

        private IObservable<TSource> WriteDeviceMetadata<TSource>(IObservable<TSource> source)
        {
            var basePath = Path;
            if (string.IsNullOrEmpty(basePath))
                return source;

            var metadataPath = System.IO.Path.Combine(basePath, MetadataFileName);
            return Observable.Create<TSource>(observer =>
            {
                Bonsai.IO.PathHelper.EnsureDirectory(metadataPath);
                if (System.IO.File.Exists(metadataPath) && !Overwrite)
                {
                    throw new System.IO.IOException(string.Format("The file '{0}' already exists.", metadataPath));
                }

                System.IO.File.WriteAllText(metadataPath, Device.Metadata);
                return source.SubscribeSafe(observer);
            });
        }

        /// <summary>
        /// Writes each Harp message in the sequence to the specified binary file, and the
        /// contents of the device metadata file to a separate text file.
        /// </summary>
        /// <param name="source">The sequence of messages to write to the file.</param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the
        /// messages to a raw binary file, and the contents of the device metadata file
        /// to a separate text file.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            return source.Publish(ps => ps.Merge(
                WriteDeviceMetadata(writer.Process(ps.GroupBy(message => message.Address)))
                .IgnoreElements()
                .Cast<HarpMessage>()));
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated from
        /// the common group register address. The contents of the device metadata file are
        /// written to a separate text file.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique register
        /// address.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the Harp
        /// messages in each group to the corresponding file, and the contents of the device
        /// metadata file to a separate text file.
        /// </returns>
        public IObservable<IGroupedObservable<int, HarpMessage>> Process(IObservable<IGroupedObservable<int, HarpMessage>> source)
        {
            return WriteDeviceMetadata(writer.Process(source));
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated from
        /// the common group register name. The contents of the device metadata file are
        /// written to a separate text file.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique register
        /// type.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the Harp
        /// messages in each group to the corresponding file, and the contents of the device
        /// metadata file to a separate text file.
        /// </returns>
        public IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<IGroupedObservable<Type, HarpMessage>> source)
        {
            return WriteDeviceMetadata(writer.Process(source));
        }
    }

    /// <summary>
    /// Represents an operator that filters register-specific messages
    /// reported by the <see cref="LaserDriverController"/> device.
    /// </summary>
    /// <seealso cref="SpadSwitch"/>
    /// <seealso cref="LaserState"/>
    /// <seealso cref="LaserFrequencySelect"/>
    /// <seealso cref="LaserIntensity"/>
    /// <seealso cref="OutputSet"/>
    /// <seealso cref="OutputClear"/>
    /// <seealso cref="OutputToggle"/>
    /// <seealso cref="OutputState"/>
    /// <seealso cref="BncsState"/>
    /// <seealso cref="SignalState"/>
    /// <seealso cref="Bnc1On"/>
    /// <seealso cref="Bnc1Off"/>
    /// <seealso cref="Bnc1Pulses"/>
    /// <seealso cref="Bnc1Tail"/>
    /// <seealso cref="Bnc2On"/>
    /// <seealso cref="Bnc2Off"/>
    /// <seealso cref="Bnc2Pulses"/>
    /// <seealso cref="Bnc2Tail"/>
    /// <seealso cref="SignalAOn"/>
    /// <seealso cref="SignalAOff"/>
    /// <seealso cref="SignalAPulses"/>
    /// <seealso cref="SignalATail"/>
    /// <seealso cref="SignalBOn"/>
    /// <seealso cref="SignalBOff"/>
    /// <seealso cref="SignalBPulses"/>
    /// <seealso cref="SignalBTail"/>
    /// <seealso cref="EventEnable"/>
    [XmlInclude(typeof(SpadSwitch))]
    [XmlInclude(typeof(LaserState))]
    [XmlInclude(typeof(LaserFrequencySelect))]
    [XmlInclude(typeof(LaserIntensity))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(BncsState))]
    [XmlInclude(typeof(SignalState))]
    [XmlInclude(typeof(Bnc1On))]
    [XmlInclude(typeof(Bnc1Off))]
    [XmlInclude(typeof(Bnc1Pulses))]
    [XmlInclude(typeof(Bnc1Tail))]
    [XmlInclude(typeof(Bnc2On))]
    [XmlInclude(typeof(Bnc2Off))]
    [XmlInclude(typeof(Bnc2Pulses))]
    [XmlInclude(typeof(Bnc2Tail))]
    [XmlInclude(typeof(SignalAOn))]
    [XmlInclude(typeof(SignalAOff))]
    [XmlInclude(typeof(SignalAPulses))]
    [XmlInclude(typeof(SignalATail))]
    [XmlInclude(typeof(SignalBOn))]
    [XmlInclude(typeof(SignalBOff))]
    [XmlInclude(typeof(SignalBPulses))]
    [XmlInclude(typeof(SignalBTail))]
    [XmlInclude(typeof(EventEnable))]
    [Description("Filters register-specific messages reported by the LaserDriverController device.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new SpadSwitch();
        }

        string INamedElement.Name
        {
            get => $"{nameof(LaserDriverController)}.{GetElementDisplayName(Register)}";
        }
    }

    /// <summary>
    /// Represents an operator which filters and selects specific messages
    /// reported by the LaserDriverController device.
    /// </summary>
    /// <seealso cref="SpadSwitch"/>
    /// <seealso cref="LaserState"/>
    /// <seealso cref="LaserFrequencySelect"/>
    /// <seealso cref="LaserIntensity"/>
    /// <seealso cref="OutputSet"/>
    /// <seealso cref="OutputClear"/>
    /// <seealso cref="OutputToggle"/>
    /// <seealso cref="OutputState"/>
    /// <seealso cref="BncsState"/>
    /// <seealso cref="SignalState"/>
    /// <seealso cref="Bnc1On"/>
    /// <seealso cref="Bnc1Off"/>
    /// <seealso cref="Bnc1Pulses"/>
    /// <seealso cref="Bnc1Tail"/>
    /// <seealso cref="Bnc2On"/>
    /// <seealso cref="Bnc2Off"/>
    /// <seealso cref="Bnc2Pulses"/>
    /// <seealso cref="Bnc2Tail"/>
    /// <seealso cref="SignalAOn"/>
    /// <seealso cref="SignalAOff"/>
    /// <seealso cref="SignalAPulses"/>
    /// <seealso cref="SignalATail"/>
    /// <seealso cref="SignalBOn"/>
    /// <seealso cref="SignalBOff"/>
    /// <seealso cref="SignalBPulses"/>
    /// <seealso cref="SignalBTail"/>
    /// <seealso cref="EventEnable"/>
    [XmlInclude(typeof(SpadSwitch))]
    [XmlInclude(typeof(LaserState))]
    [XmlInclude(typeof(LaserFrequencySelect))]
    [XmlInclude(typeof(LaserIntensity))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(BncsState))]
    [XmlInclude(typeof(SignalState))]
    [XmlInclude(typeof(Bnc1On))]
    [XmlInclude(typeof(Bnc1Off))]
    [XmlInclude(typeof(Bnc1Pulses))]
    [XmlInclude(typeof(Bnc1Tail))]
    [XmlInclude(typeof(Bnc2On))]
    [XmlInclude(typeof(Bnc2Off))]
    [XmlInclude(typeof(Bnc2Pulses))]
    [XmlInclude(typeof(Bnc2Tail))]
    [XmlInclude(typeof(SignalAOn))]
    [XmlInclude(typeof(SignalAOff))]
    [XmlInclude(typeof(SignalAPulses))]
    [XmlInclude(typeof(SignalATail))]
    [XmlInclude(typeof(SignalBOn))]
    [XmlInclude(typeof(SignalBOff))]
    [XmlInclude(typeof(SignalBPulses))]
    [XmlInclude(typeof(SignalBTail))]
    [XmlInclude(typeof(EventEnable))]
    [XmlInclude(typeof(TimestampedSpadSwitch))]
    [XmlInclude(typeof(TimestampedLaserState))]
    [XmlInclude(typeof(TimestampedLaserFrequencySelect))]
    [XmlInclude(typeof(TimestampedLaserIntensity))]
    [XmlInclude(typeof(TimestampedOutputSet))]
    [XmlInclude(typeof(TimestampedOutputClear))]
    [XmlInclude(typeof(TimestampedOutputToggle))]
    [XmlInclude(typeof(TimestampedOutputState))]
    [XmlInclude(typeof(TimestampedBncsState))]
    [XmlInclude(typeof(TimestampedSignalState))]
    [XmlInclude(typeof(TimestampedBnc1On))]
    [XmlInclude(typeof(TimestampedBnc1Off))]
    [XmlInclude(typeof(TimestampedBnc1Pulses))]
    [XmlInclude(typeof(TimestampedBnc1Tail))]
    [XmlInclude(typeof(TimestampedBnc2On))]
    [XmlInclude(typeof(TimestampedBnc2Off))]
    [XmlInclude(typeof(TimestampedBnc2Pulses))]
    [XmlInclude(typeof(TimestampedBnc2Tail))]
    [XmlInclude(typeof(TimestampedSignalAOn))]
    [XmlInclude(typeof(TimestampedSignalAOff))]
    [XmlInclude(typeof(TimestampedSignalAPulses))]
    [XmlInclude(typeof(TimestampedSignalATail))]
    [XmlInclude(typeof(TimestampedSignalBOn))]
    [XmlInclude(typeof(TimestampedSignalBOff))]
    [XmlInclude(typeof(TimestampedSignalBPulses))]
    [XmlInclude(typeof(TimestampedSignalBTail))]
    [XmlInclude(typeof(TimestampedEventEnable))]
    [Description("Filters and selects specific messages reported by the LaserDriverController device.")]
    public partial class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new SpadSwitch();
        }

        string INamedElement.Name => $"{nameof(LaserDriverController)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// LaserDriverController register messages.
    /// </summary>
    /// <seealso cref="SpadSwitch"/>
    /// <seealso cref="LaserState"/>
    /// <seealso cref="LaserFrequencySelect"/>
    /// <seealso cref="LaserIntensity"/>
    /// <seealso cref="OutputSet"/>
    /// <seealso cref="OutputClear"/>
    /// <seealso cref="OutputToggle"/>
    /// <seealso cref="OutputState"/>
    /// <seealso cref="BncsState"/>
    /// <seealso cref="SignalState"/>
    /// <seealso cref="Bnc1On"/>
    /// <seealso cref="Bnc1Off"/>
    /// <seealso cref="Bnc1Pulses"/>
    /// <seealso cref="Bnc1Tail"/>
    /// <seealso cref="Bnc2On"/>
    /// <seealso cref="Bnc2Off"/>
    /// <seealso cref="Bnc2Pulses"/>
    /// <seealso cref="Bnc2Tail"/>
    /// <seealso cref="SignalAOn"/>
    /// <seealso cref="SignalAOff"/>
    /// <seealso cref="SignalAPulses"/>
    /// <seealso cref="SignalATail"/>
    /// <seealso cref="SignalBOn"/>
    /// <seealso cref="SignalBOff"/>
    /// <seealso cref="SignalBPulses"/>
    /// <seealso cref="SignalBTail"/>
    /// <seealso cref="EventEnable"/>
    [XmlInclude(typeof(SpadSwitch))]
    [XmlInclude(typeof(LaserState))]
    [XmlInclude(typeof(LaserFrequencySelect))]
    [XmlInclude(typeof(LaserIntensity))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(BncsState))]
    [XmlInclude(typeof(SignalState))]
    [XmlInclude(typeof(Bnc1On))]
    [XmlInclude(typeof(Bnc1Off))]
    [XmlInclude(typeof(Bnc1Pulses))]
    [XmlInclude(typeof(Bnc1Tail))]
    [XmlInclude(typeof(Bnc2On))]
    [XmlInclude(typeof(Bnc2Off))]
    [XmlInclude(typeof(Bnc2Pulses))]
    [XmlInclude(typeof(Bnc2Tail))]
    [XmlInclude(typeof(SignalAOn))]
    [XmlInclude(typeof(SignalAOff))]
    [XmlInclude(typeof(SignalAPulses))]
    [XmlInclude(typeof(SignalATail))]
    [XmlInclude(typeof(SignalBOn))]
    [XmlInclude(typeof(SignalBOff))]
    [XmlInclude(typeof(SignalBPulses))]
    [XmlInclude(typeof(SignalBTail))]
    [XmlInclude(typeof(EventEnable))]
    [Description("Formats a sequence of values as specific LaserDriverController register messages.")]
    public partial class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new SpadSwitch();
        }

        string INamedElement.Name => $"{nameof(LaserDriverController)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents a register that turns ON/OFF the relay to switch SPADs supply.
    /// </summary>
    [Description("Turns ON/OFF the relay to switch SPADs supply")]
    public partial class SpadSwitch
    {
        /// <summary>
        /// Represents the address of the <see cref="SpadSwitch"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="SpadSwitch"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="SpadSwitch"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SpadSwitch"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SpadSwitch"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SpadSwitch"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SpadSwitch"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SpadSwitch"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SpadSwitch"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SpadSwitch register.
    /// </summary>
    /// <seealso cref="SpadSwitch"/>
    [Description("Filters and selects timestamped messages from the SpadSwitch register.")]
    public partial class TimestampedSpadSwitch
    {
        /// <summary>
        /// Represents the address of the <see cref="SpadSwitch"/> register. This field is constant.
        /// </summary>
        public const int Address = SpadSwitch.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SpadSwitch"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return SpadSwitch.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that state of the laser, ON/OFF.
    /// </summary>
    [Description("State of the laser, ON/OFF")]
    public partial class LaserState
    {
        /// <summary>
        /// Represents the address of the <see cref="LaserState"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

        /// <summary>
        /// Represents the payload type of the <see cref="LaserState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="LaserState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LaserState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LaserState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LaserState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LaserState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LaserState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LaserState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LaserState register.
    /// </summary>
    /// <seealso cref="LaserState"/>
    [Description("Filters and selects timestamped messages from the LaserState register.")]
    public partial class TimestampedLaserState
    {
        /// <summary>
        /// Represents the address of the <see cref="LaserState"/> register. This field is constant.
        /// </summary>
        public const int Address = LaserState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LaserState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return LaserState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that reserved for future use.
    /// </summary>
    [Description("Reserved for future use")]
    internal partial class Reserved0
    {
        /// <summary>
        /// Represents the address of the <see cref="Reserved0"/> register. This field is constant.
        /// </summary>
        public const int Address = 34;

        /// <summary>
        /// Represents the payload type of the <see cref="Reserved0"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Reserved0"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;
    }

    /// <summary>
    /// Represents a register that reserved for future use.
    /// </summary>
    [Description("Reserved for future use")]
    internal partial class Reserved1
    {
        /// <summary>
        /// Represents the address of the <see cref="Reserved1"/> register. This field is constant.
        /// </summary>
        public const int Address = 35;

        /// <summary>
        /// Represents the payload type of the <see cref="Reserved1"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Reserved1"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;
    }

    /// <summary>
    /// Represents a register that reserved for future use.
    /// </summary>
    [Description("Reserved for future use")]
    internal partial class Reserved2
    {
        /// <summary>
        /// Represents the address of the <see cref="Reserved2"/> register. This field is constant.
        /// </summary>
        public const int Address = 36;

        /// <summary>
        /// Represents the payload type of the <see cref="Reserved2"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Reserved2"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;
    }

    /// <summary>
    /// Represents a register that reserved for future use.
    /// </summary>
    [Description("Reserved for future use")]
    internal partial class Reserved13
    {
        /// <summary>
        /// Represents the address of the <see cref="Reserved13"/> register. This field is constant.
        /// </summary>
        public const int Address = 37;

        /// <summary>
        /// Represents the payload type of the <see cref="Reserved13"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Reserved13"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;
    }

    /// <summary>
    /// Represents a register that set the laser frequency.
    /// </summary>
    [Description("Set the laser frequency")]
    public partial class LaserFrequencySelect
    {
        /// <summary>
        /// Represents the address of the <see cref="LaserFrequencySelect"/> register. This field is constant.
        /// </summary>
        public const int Address = 38;

        /// <summary>
        /// Represents the payload type of the <see cref="LaserFrequencySelect"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="LaserFrequencySelect"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LaserFrequencySelect"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static FrequencySelect GetPayload(HarpMessage message)
        {
            return (FrequencySelect)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LaserFrequencySelect"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<FrequencySelect> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((FrequencySelect)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LaserFrequencySelect"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LaserFrequencySelect"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, FrequencySelect value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LaserFrequencySelect"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LaserFrequencySelect"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, FrequencySelect value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LaserFrequencySelect register.
    /// </summary>
    /// <seealso cref="LaserFrequencySelect"/>
    [Description("Filters and selects timestamped messages from the LaserFrequencySelect register.")]
    public partial class TimestampedLaserFrequencySelect
    {
        /// <summary>
        /// Represents the address of the <see cref="LaserFrequencySelect"/> register. This field is constant.
        /// </summary>
        public const int Address = LaserFrequencySelect.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LaserFrequencySelect"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<FrequencySelect> GetPayload(HarpMessage message)
        {
            return LaserFrequencySelect.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that laser intensity value [0:255].
    /// </summary>
    [Description("Laser intensity value [0:255]")]
    public partial class LaserIntensity
    {
        /// <summary>
        /// Represents the address of the <see cref="LaserIntensity"/> register. This field is constant.
        /// </summary>
        public const int Address = 39;

        /// <summary>
        /// Represents the payload type of the <see cref="LaserIntensity"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="LaserIntensity"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LaserIntensity"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LaserIntensity"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LaserIntensity"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LaserIntensity"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LaserIntensity"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LaserIntensity"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LaserIntensity register.
    /// </summary>
    /// <seealso cref="LaserIntensity"/>
    [Description("Filters and selects timestamped messages from the LaserIntensity register.")]
    public partial class TimestampedLaserIntensity
    {
        /// <summary>
        /// Represents the address of the <see cref="LaserIntensity"/> register. This field is constant.
        /// </summary>
        public const int Address = LaserIntensity.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LaserIntensity"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return LaserIntensity.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the specified digital output lines.
    /// </summary>
    [Description("Set the specified digital output lines")]
    public partial class OutputSet
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputSet"/> register. This field is constant.
        /// </summary>
        public const int Address = 40;

        /// <summary>
        /// Represents the payload type of the <see cref="OutputSet"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="OutputSet"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OutputSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalOutputs GetPayload(HarpMessage message)
        {
            return (DigitalOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((DigitalOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OutputSet"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputSet"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OutputSet"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputSet"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OutputSet register.
    /// </summary>
    /// <seealso cref="OutputSet"/>
    [Description("Filters and selects timestamped messages from the OutputSet register.")]
    public partial class TimestampedOutputSet
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputSet"/> register. This field is constant.
        /// </summary>
        public const int Address = OutputSet.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OutputSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetPayload(HarpMessage message)
        {
            return OutputSet.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that clear the specified digital output lines.
    /// </summary>
    [Description("Clear the specified digital output lines")]
    public partial class OutputClear
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputClear"/> register. This field is constant.
        /// </summary>
        public const int Address = 41;

        /// <summary>
        /// Represents the payload type of the <see cref="OutputClear"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="OutputClear"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OutputClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalOutputs GetPayload(HarpMessage message)
        {
            return (DigitalOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((DigitalOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OutputClear"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputClear"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OutputClear"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputClear"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OutputClear register.
    /// </summary>
    /// <seealso cref="OutputClear"/>
    [Description("Filters and selects timestamped messages from the OutputClear register.")]
    public partial class TimestampedOutputClear
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputClear"/> register. This field is constant.
        /// </summary>
        public const int Address = OutputClear.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OutputClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetPayload(HarpMessage message)
        {
            return OutputClear.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that toggle the specified digital output lines.
    /// </summary>
    [Description("Toggle the specified digital output lines")]
    public partial class OutputToggle
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputToggle"/> register. This field is constant.
        /// </summary>
        public const int Address = 42;

        /// <summary>
        /// Represents the payload type of the <see cref="OutputToggle"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="OutputToggle"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OutputToggle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalOutputs GetPayload(HarpMessage message)
        {
            return (DigitalOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputToggle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((DigitalOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OutputToggle"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputToggle"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OutputToggle"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputToggle"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OutputToggle register.
    /// </summary>
    /// <seealso cref="OutputToggle"/>
    [Description("Filters and selects timestamped messages from the OutputToggle register.")]
    public partial class TimestampedOutputToggle
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputToggle"/> register. This field is constant.
        /// </summary>
        public const int Address = OutputToggle.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OutputToggle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetPayload(HarpMessage message)
        {
            return OutputToggle.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that write the state of all digital output lines.
    /// </summary>
    [Description("Write the state of all digital output lines")]
    public partial class OutputState
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputState"/> register. This field is constant.
        /// </summary>
        public const int Address = 43;

        /// <summary>
        /// Represents the payload type of the <see cref="OutputState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="OutputState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OutputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalOutputs GetPayload(HarpMessage message)
        {
            return (DigitalOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((DigitalOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OutputState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OutputState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OutputState register.
    /// </summary>
    /// <seealso cref="OutputState"/>
    [Description("Filters and selects timestamped messages from the OutputState register.")]
    public partial class TimestampedOutputState
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputState"/> register. This field is constant.
        /// </summary>
        public const int Address = OutputState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OutputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetPayload(HarpMessage message)
        {
            return OutputState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configure BNCs to start.
    /// </summary>
    [Description("Configure BNCs to start")]
    public partial class BncsState
    {
        /// <summary>
        /// Represents the address of the <see cref="BncsState"/> register. This field is constant.
        /// </summary>
        public const int Address = 44;

        /// <summary>
        /// Represents the payload type of the <see cref="BncsState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="BncsState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="BncsState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Bncs GetPayload(HarpMessage message)
        {
            return (Bncs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="BncsState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Bncs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Bncs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="BncsState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="BncsState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Bncs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="BncsState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="BncsState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Bncs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// BncsState register.
    /// </summary>
    /// <seealso cref="BncsState"/>
    [Description("Filters and selects timestamped messages from the BncsState register.")]
    public partial class TimestampedBncsState
    {
        /// <summary>
        /// Represents the address of the <see cref="BncsState"/> register. This field is constant.
        /// </summary>
        public const int Address = BncsState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="BncsState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Bncs> GetPayload(HarpMessage message)
        {
            return BncsState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configure Signals to start.
    /// </summary>
    [Description("Configure Signals to start")]
    public partial class SignalState
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalState"/> register. This field is constant.
        /// </summary>
        public const int Address = 45;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="SignalState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Signals GetPayload(HarpMessage message)
        {
            return (Signals)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Signals> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Signals)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Signals value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Signals value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalState register.
    /// </summary>
    /// <seealso cref="SignalState"/>
    [Description("Filters and selects timestamped messages from the SignalState register.")]
    public partial class TimestampedSignalState
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalState"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Signals> GetPayload(HarpMessage message)
        {
            return SignalState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time ON of BNC1 (milliseconds) [1:65535].
    /// </summary>
    [Description("Time ON of BNC1 (milliseconds) [1:65535]")]
    public partial class Bnc1On
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc1On"/> register. This field is constant.
        /// </summary>
        public const int Address = 46;

        /// <summary>
        /// Represents the payload type of the <see cref="Bnc1On"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Bnc1On"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Bnc1On"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Bnc1On"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Bnc1On"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc1On"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Bnc1On"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc1On"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Bnc1On register.
    /// </summary>
    /// <seealso cref="Bnc1On"/>
    [Description("Filters and selects timestamped messages from the Bnc1On register.")]
    public partial class TimestampedBnc1On
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc1On"/> register. This field is constant.
        /// </summary>
        public const int Address = Bnc1On.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Bnc1On"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Bnc1On.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time OFF of BNC1 (milliseconds) [1:65535].
    /// </summary>
    [Description("Time OFF of BNC1 (milliseconds) [1:65535]")]
    public partial class Bnc1Off
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc1Off"/> register. This field is constant.
        /// </summary>
        public const int Address = 47;

        /// <summary>
        /// Represents the payload type of the <see cref="Bnc1Off"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Bnc1Off"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Bnc1Off"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Bnc1Off"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Bnc1Off"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc1Off"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Bnc1Off"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc1Off"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Bnc1Off register.
    /// </summary>
    /// <seealso cref="Bnc1Off"/>
    [Description("Filters and selects timestamped messages from the Bnc1Off register.")]
    public partial class TimestampedBnc1Off
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc1Off"/> register. This field is constant.
        /// </summary>
        public const int Address = Bnc1Off.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Bnc1Off"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Bnc1Off.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that number of pulses (BNC1) [0;65535], 0-> infinite repeat.
    /// </summary>
    [Description("Number of pulses (BNC1) [0;65535], 0-> infinite repeat")]
    public partial class Bnc1Pulses
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc1Pulses"/> register. This field is constant.
        /// </summary>
        public const int Address = 48;

        /// <summary>
        /// Represents the payload type of the <see cref="Bnc1Pulses"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Bnc1Pulses"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Bnc1Pulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Bnc1Pulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Bnc1Pulses"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc1Pulses"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Bnc1Pulses"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc1Pulses"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Bnc1Pulses register.
    /// </summary>
    /// <seealso cref="Bnc1Pulses"/>
    [Description("Filters and selects timestamped messages from the Bnc1Pulses register.")]
    public partial class TimestampedBnc1Pulses
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc1Pulses"/> register. This field is constant.
        /// </summary>
        public const int Address = Bnc1Pulses.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Bnc1Pulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Bnc1Pulses.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that wait time to start (milliseconds) (BNC1) [1;65535].
    /// </summary>
    [Description("Wait time to start (milliseconds) (BNC1) [1;65535]")]
    public partial class Bnc1Tail
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc1Tail"/> register. This field is constant.
        /// </summary>
        public const int Address = 49;

        /// <summary>
        /// Represents the payload type of the <see cref="Bnc1Tail"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Bnc1Tail"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Bnc1Tail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Bnc1Tail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Bnc1Tail"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc1Tail"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Bnc1Tail"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc1Tail"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Bnc1Tail register.
    /// </summary>
    /// <seealso cref="Bnc1Tail"/>
    [Description("Filters and selects timestamped messages from the Bnc1Tail register.")]
    public partial class TimestampedBnc1Tail
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc1Tail"/> register. This field is constant.
        /// </summary>
        public const int Address = Bnc1Tail.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Bnc1Tail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Bnc1Tail.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time ON of BNC2 (milliseconds) [1:65535].
    /// </summary>
    [Description("Time ON of BNC2 (milliseconds) [1:65535]")]
    public partial class Bnc2On
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc2On"/> register. This field is constant.
        /// </summary>
        public const int Address = 50;

        /// <summary>
        /// Represents the payload type of the <see cref="Bnc2On"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Bnc2On"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Bnc2On"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Bnc2On"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Bnc2On"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc2On"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Bnc2On"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc2On"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Bnc2On register.
    /// </summary>
    /// <seealso cref="Bnc2On"/>
    [Description("Filters and selects timestamped messages from the Bnc2On register.")]
    public partial class TimestampedBnc2On
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc2On"/> register. This field is constant.
        /// </summary>
        public const int Address = Bnc2On.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Bnc2On"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Bnc2On.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time OFF of BNC2 (milliseconds) [1:65535].
    /// </summary>
    [Description("Time OFF of BNC2 (milliseconds) [1:65535]")]
    public partial class Bnc2Off
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc2Off"/> register. This field is constant.
        /// </summary>
        public const int Address = 51;

        /// <summary>
        /// Represents the payload type of the <see cref="Bnc2Off"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Bnc2Off"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Bnc2Off"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Bnc2Off"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Bnc2Off"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc2Off"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Bnc2Off"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc2Off"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Bnc2Off register.
    /// </summary>
    /// <seealso cref="Bnc2Off"/>
    [Description("Filters and selects timestamped messages from the Bnc2Off register.")]
    public partial class TimestampedBnc2Off
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc2Off"/> register. This field is constant.
        /// </summary>
        public const int Address = Bnc2Off.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Bnc2Off"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Bnc2Off.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that number of pulses (BNC2) [0;65535], 0-> infinite repeat.
    /// </summary>
    [Description("Number of pulses (BNC2) [0;65535], 0-> infinite repeat")]
    public partial class Bnc2Pulses
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc2Pulses"/> register. This field is constant.
        /// </summary>
        public const int Address = 52;

        /// <summary>
        /// Represents the payload type of the <see cref="Bnc2Pulses"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Bnc2Pulses"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Bnc2Pulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Bnc2Pulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Bnc2Pulses"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc2Pulses"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Bnc2Pulses"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc2Pulses"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Bnc2Pulses register.
    /// </summary>
    /// <seealso cref="Bnc2Pulses"/>
    [Description("Filters and selects timestamped messages from the Bnc2Pulses register.")]
    public partial class TimestampedBnc2Pulses
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc2Pulses"/> register. This field is constant.
        /// </summary>
        public const int Address = Bnc2Pulses.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Bnc2Pulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Bnc2Pulses.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that wait time to start (milliseconds) (BNC2) [1;65535].
    /// </summary>
    [Description("Wait time to start (milliseconds) (BNC2) [1;65535]")]
    public partial class Bnc2Tail
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc2Tail"/> register. This field is constant.
        /// </summary>
        public const int Address = 53;

        /// <summary>
        /// Represents the payload type of the <see cref="Bnc2Tail"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Bnc2Tail"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Bnc2Tail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Bnc2Tail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Bnc2Tail"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc2Tail"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Bnc2Tail"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Bnc2Tail"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Bnc2Tail register.
    /// </summary>
    /// <seealso cref="Bnc2Tail"/>
    [Description("Filters and selects timestamped messages from the Bnc2Tail register.")]
    public partial class TimestampedBnc2Tail
    {
        /// <summary>
        /// Represents the address of the <see cref="Bnc2Tail"/> register. This field is constant.
        /// </summary>
        public const int Address = Bnc2Tail.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Bnc2Tail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Bnc2Tail.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time ON of SignalA (milliseconds) [1:65535].
    /// </summary>
    [Description("Time ON of SignalA (milliseconds) [1:65535]")]
    public partial class SignalAOn
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalAOn"/> register. This field is constant.
        /// </summary>
        public const int Address = 54;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalAOn"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SignalAOn"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalAOn"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalAOn"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalAOn"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalAOn"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalAOn"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalAOn"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalAOn register.
    /// </summary>
    /// <seealso cref="SignalAOn"/>
    [Description("Filters and selects timestamped messages from the SignalAOn register.")]
    public partial class TimestampedSignalAOn
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalAOn"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalAOn.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalAOn"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SignalAOn.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time OFF of SignalA (milliseconds) [1:65535].
    /// </summary>
    [Description("Time OFF of SignalA (milliseconds) [1:65535]")]
    public partial class SignalAOff
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalAOff"/> register. This field is constant.
        /// </summary>
        public const int Address = 55;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalAOff"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SignalAOff"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalAOff"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalAOff"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalAOff"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalAOff"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalAOff"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalAOff"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalAOff register.
    /// </summary>
    /// <seealso cref="SignalAOff"/>
    [Description("Filters and selects timestamped messages from the SignalAOff register.")]
    public partial class TimestampedSignalAOff
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalAOff"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalAOff.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalAOff"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SignalAOff.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that number of pulses (SignalA) [0;65535], 0-> infinite repeat.
    /// </summary>
    [Description("Number of pulses (SignalA) [0;65535], 0-> infinite repeat")]
    public partial class SignalAPulses
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalAPulses"/> register. This field is constant.
        /// </summary>
        public const int Address = 56;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalAPulses"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SignalAPulses"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalAPulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalAPulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalAPulses"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalAPulses"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalAPulses"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalAPulses"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalAPulses register.
    /// </summary>
    /// <seealso cref="SignalAPulses"/>
    [Description("Filters and selects timestamped messages from the SignalAPulses register.")]
    public partial class TimestampedSignalAPulses
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalAPulses"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalAPulses.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalAPulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SignalAPulses.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that wait time to start (milliseconds) (SignalA) [1;65535].
    /// </summary>
    [Description("Wait time to start (milliseconds) (SignalA) [1;65535]")]
    public partial class SignalATail
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalATail"/> register. This field is constant.
        /// </summary>
        public const int Address = 57;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalATail"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SignalATail"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalATail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalATail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalATail"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalATail"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalATail"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalATail"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalATail register.
    /// </summary>
    /// <seealso cref="SignalATail"/>
    [Description("Filters and selects timestamped messages from the SignalATail register.")]
    public partial class TimestampedSignalATail
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalATail"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalATail.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalATail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SignalATail.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time ON of SignalB (milliseconds) [1:65535].
    /// </summary>
    [Description("Time ON of SignalB (milliseconds) [1:65535]")]
    public partial class SignalBOn
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalBOn"/> register. This field is constant.
        /// </summary>
        public const int Address = 58;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalBOn"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SignalBOn"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalBOn"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalBOn"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalBOn"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalBOn"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalBOn"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalBOn"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalBOn register.
    /// </summary>
    /// <seealso cref="SignalBOn"/>
    [Description("Filters and selects timestamped messages from the SignalBOn register.")]
    public partial class TimestampedSignalBOn
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalBOn"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalBOn.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalBOn"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SignalBOn.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that time OFF of SignalB (milliseconds) [1:65535].
    /// </summary>
    [Description("Time OFF of SignalB (milliseconds) [1:65535]")]
    public partial class SignalBOff
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalBOff"/> register. This field is constant.
        /// </summary>
        public const int Address = 59;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalBOff"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SignalBOff"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalBOff"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalBOff"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalBOff"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalBOff"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalBOff"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalBOff"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalBOff register.
    /// </summary>
    /// <seealso cref="SignalBOff"/>
    [Description("Filters and selects timestamped messages from the SignalBOff register.")]
    public partial class TimestampedSignalBOff
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalBOff"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalBOff.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalBOff"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SignalBOff.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that number of pulses (SignalB) [0;65535], 0-> infinite repeat.
    /// </summary>
    [Description("Number of pulses (SignalB) [0;65535], 0-> infinite repeat")]
    public partial class SignalBPulses
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalBPulses"/> register. This field is constant.
        /// </summary>
        public const int Address = 60;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalBPulses"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SignalBPulses"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalBPulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalBPulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalBPulses"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalBPulses"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalBPulses"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalBPulses"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalBPulses register.
    /// </summary>
    /// <seealso cref="SignalBPulses"/>
    [Description("Filters and selects timestamped messages from the SignalBPulses register.")]
    public partial class TimestampedSignalBPulses
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalBPulses"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalBPulses.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalBPulses"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SignalBPulses.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that wait time to start (milliseconds) (SignalB) [1;65535].
    /// </summary>
    [Description("Wait time to start (milliseconds) (SignalB) [1;65535]")]
    public partial class SignalBTail
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalBTail"/> register. This field is constant.
        /// </summary>
        public const int Address = 61;

        /// <summary>
        /// Represents the payload type of the <see cref="SignalBTail"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SignalBTail"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SignalBTail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SignalBTail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SignalBTail"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalBTail"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SignalBTail"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SignalBTail"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SignalBTail register.
    /// </summary>
    /// <seealso cref="SignalBTail"/>
    [Description("Filters and selects timestamped messages from the SignalBTail register.")]
    public partial class TimestampedSignalBTail
    {
        /// <summary>
        /// Represents the address of the <see cref="SignalBTail"/> register. This field is constant.
        /// </summary>
        public const int Address = SignalBTail.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SignalBTail"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SignalBTail.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the active events in the device.
    /// </summary>
    [Description("Specifies the active events in the device")]
    public partial class EventEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="EventEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = 62;

        /// <summary>
        /// Represents the payload type of the <see cref="EventEnable"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EventEnable"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EventEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LaserDriverControllerEvents GetPayload(HarpMessage message)
        {
            return (LaserDriverControllerEvents)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EventEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LaserDriverControllerEvents> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LaserDriverControllerEvents)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EventEnable"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EventEnable"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LaserDriverControllerEvents value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EventEnable"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EventEnable"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LaserDriverControllerEvents value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EventEnable register.
    /// </summary>
    /// <seealso cref="EventEnable"/>
    [Description("Filters and selects timestamped messages from the EventEnable register.")]
    public partial class TimestampedEventEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="EventEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = EventEnable.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EventEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LaserDriverControllerEvents> GetPayload(HarpMessage message)
        {
            return EventEnable.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// LaserDriverController device.
    /// </summary>
    /// <seealso cref="CreateSpadSwitchPayload"/>
    /// <seealso cref="CreateLaserStatePayload"/>
    /// <seealso cref="CreateLaserFrequencySelectPayload"/>
    /// <seealso cref="CreateLaserIntensityPayload"/>
    /// <seealso cref="CreateOutputSetPayload"/>
    /// <seealso cref="CreateOutputClearPayload"/>
    /// <seealso cref="CreateOutputTogglePayload"/>
    /// <seealso cref="CreateOutputStatePayload"/>
    /// <seealso cref="CreateBncsStatePayload"/>
    /// <seealso cref="CreateSignalStatePayload"/>
    /// <seealso cref="CreateBnc1OnPayload"/>
    /// <seealso cref="CreateBnc1OffPayload"/>
    /// <seealso cref="CreateBnc1PulsesPayload"/>
    /// <seealso cref="CreateBnc1TailPayload"/>
    /// <seealso cref="CreateBnc2OnPayload"/>
    /// <seealso cref="CreateBnc2OffPayload"/>
    /// <seealso cref="CreateBnc2PulsesPayload"/>
    /// <seealso cref="CreateBnc2TailPayload"/>
    /// <seealso cref="CreateSignalAOnPayload"/>
    /// <seealso cref="CreateSignalAOffPayload"/>
    /// <seealso cref="CreateSignalAPulsesPayload"/>
    /// <seealso cref="CreateSignalATailPayload"/>
    /// <seealso cref="CreateSignalBOnPayload"/>
    /// <seealso cref="CreateSignalBOffPayload"/>
    /// <seealso cref="CreateSignalBPulsesPayload"/>
    /// <seealso cref="CreateSignalBTailPayload"/>
    /// <seealso cref="CreateEventEnablePayload"/>
    [XmlInclude(typeof(CreateSpadSwitchPayload))]
    [XmlInclude(typeof(CreateLaserStatePayload))]
    [XmlInclude(typeof(CreateLaserFrequencySelectPayload))]
    [XmlInclude(typeof(CreateLaserIntensityPayload))]
    [XmlInclude(typeof(CreateOutputSetPayload))]
    [XmlInclude(typeof(CreateOutputClearPayload))]
    [XmlInclude(typeof(CreateOutputTogglePayload))]
    [XmlInclude(typeof(CreateOutputStatePayload))]
    [XmlInclude(typeof(CreateBncsStatePayload))]
    [XmlInclude(typeof(CreateSignalStatePayload))]
    [XmlInclude(typeof(CreateBnc1OnPayload))]
    [XmlInclude(typeof(CreateBnc1OffPayload))]
    [XmlInclude(typeof(CreateBnc1PulsesPayload))]
    [XmlInclude(typeof(CreateBnc1TailPayload))]
    [XmlInclude(typeof(CreateBnc2OnPayload))]
    [XmlInclude(typeof(CreateBnc2OffPayload))]
    [XmlInclude(typeof(CreateBnc2PulsesPayload))]
    [XmlInclude(typeof(CreateBnc2TailPayload))]
    [XmlInclude(typeof(CreateSignalAOnPayload))]
    [XmlInclude(typeof(CreateSignalAOffPayload))]
    [XmlInclude(typeof(CreateSignalAPulsesPayload))]
    [XmlInclude(typeof(CreateSignalATailPayload))]
    [XmlInclude(typeof(CreateSignalBOnPayload))]
    [XmlInclude(typeof(CreateSignalBOffPayload))]
    [XmlInclude(typeof(CreateSignalBPulsesPayload))]
    [XmlInclude(typeof(CreateSignalBTailPayload))]
    [XmlInclude(typeof(CreateEventEnablePayload))]
    [XmlInclude(typeof(CreateTimestampedSpadSwitchPayload))]
    [XmlInclude(typeof(CreateTimestampedLaserStatePayload))]
    [XmlInclude(typeof(CreateTimestampedLaserFrequencySelectPayload))]
    [XmlInclude(typeof(CreateTimestampedLaserIntensityPayload))]
    [XmlInclude(typeof(CreateTimestampedOutputSetPayload))]
    [XmlInclude(typeof(CreateTimestampedOutputClearPayload))]
    [XmlInclude(typeof(CreateTimestampedOutputTogglePayload))]
    [XmlInclude(typeof(CreateTimestampedOutputStatePayload))]
    [XmlInclude(typeof(CreateTimestampedBncsStatePayload))]
    [XmlInclude(typeof(CreateTimestampedSignalStatePayload))]
    [XmlInclude(typeof(CreateTimestampedBnc1OnPayload))]
    [XmlInclude(typeof(CreateTimestampedBnc1OffPayload))]
    [XmlInclude(typeof(CreateTimestampedBnc1PulsesPayload))]
    [XmlInclude(typeof(CreateTimestampedBnc1TailPayload))]
    [XmlInclude(typeof(CreateTimestampedBnc2OnPayload))]
    [XmlInclude(typeof(CreateTimestampedBnc2OffPayload))]
    [XmlInclude(typeof(CreateTimestampedBnc2PulsesPayload))]
    [XmlInclude(typeof(CreateTimestampedBnc2TailPayload))]
    [XmlInclude(typeof(CreateTimestampedSignalAOnPayload))]
    [XmlInclude(typeof(CreateTimestampedSignalAOffPayload))]
    [XmlInclude(typeof(CreateTimestampedSignalAPulsesPayload))]
    [XmlInclude(typeof(CreateTimestampedSignalATailPayload))]
    [XmlInclude(typeof(CreateTimestampedSignalBOnPayload))]
    [XmlInclude(typeof(CreateTimestampedSignalBOffPayload))]
    [XmlInclude(typeof(CreateTimestampedSignalBPulsesPayload))]
    [XmlInclude(typeof(CreateTimestampedSignalBTailPayload))]
    [XmlInclude(typeof(CreateTimestampedEventEnablePayload))]
    [Description("Creates standard message payloads for the LaserDriverController device.")]
    public partial class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreateSpadSwitchPayload();
        }

        string INamedElement.Name => $"{nameof(LaserDriverController)}.{GetElementDisplayName(Payload)}";
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that turns ON/OFF the relay to switch SPADs supply.
    /// </summary>
    [DisplayName("SpadSwitchPayload")]
    [Description("Creates a message payload that turns ON/OFF the relay to switch SPADs supply.")]
    public partial class CreateSpadSwitchPayload
    {
        /// <summary>
        /// Gets or sets the value that turns ON/OFF the relay to switch SPADs supply.
        /// </summary>
        [Description("The value that turns ON/OFF the relay to switch SPADs supply.")]
        public byte SpadSwitch { get; set; }

        /// <summary>
        /// Creates a message payload for the SpadSwitch register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return SpadSwitch;
        }

        /// <summary>
        /// Creates a message that turns ON/OFF the relay to switch SPADs supply.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SpadSwitch register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SpadSwitch.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that turns ON/OFF the relay to switch SPADs supply.
    /// </summary>
    [DisplayName("TimestampedSpadSwitchPayload")]
    [Description("Creates a timestamped message payload that turns ON/OFF the relay to switch SPADs supply.")]
    public partial class CreateTimestampedSpadSwitchPayload : CreateSpadSwitchPayload
    {
        /// <summary>
        /// Creates a timestamped message that turns ON/OFF the relay to switch SPADs supply.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SpadSwitch register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SpadSwitch.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that state of the laser, ON/OFF.
    /// </summary>
    [DisplayName("LaserStatePayload")]
    [Description("Creates a message payload that state of the laser, ON/OFF.")]
    public partial class CreateLaserStatePayload
    {
        /// <summary>
        /// Gets or sets the value that state of the laser, ON/OFF.
        /// </summary>
        [Description("The value that state of the laser, ON/OFF.")]
        public byte LaserState { get; set; }

        /// <summary>
        /// Creates a message payload for the LaserState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return LaserState;
        }

        /// <summary>
        /// Creates a message that state of the laser, ON/OFF.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LaserState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.LaserState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that state of the laser, ON/OFF.
    /// </summary>
    [DisplayName("TimestampedLaserStatePayload")]
    [Description("Creates a timestamped message payload that state of the laser, ON/OFF.")]
    public partial class CreateTimestampedLaserStatePayload : CreateLaserStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that state of the laser, ON/OFF.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LaserState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.LaserState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the laser frequency.
    /// </summary>
    [DisplayName("LaserFrequencySelectPayload")]
    [Description("Creates a message payload that set the laser frequency.")]
    public partial class CreateLaserFrequencySelectPayload
    {
        /// <summary>
        /// Gets or sets the value that set the laser frequency.
        /// </summary>
        [Description("The value that set the laser frequency.")]
        public FrequencySelect LaserFrequencySelect { get; set; }

        /// <summary>
        /// Creates a message payload for the LaserFrequencySelect register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public FrequencySelect GetPayload()
        {
            return LaserFrequencySelect;
        }

        /// <summary>
        /// Creates a message that set the laser frequency.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LaserFrequencySelect register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.LaserFrequencySelect.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the laser frequency.
    /// </summary>
    [DisplayName("TimestampedLaserFrequencySelectPayload")]
    [Description("Creates a timestamped message payload that set the laser frequency.")]
    public partial class CreateTimestampedLaserFrequencySelectPayload : CreateLaserFrequencySelectPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the laser frequency.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LaserFrequencySelect register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.LaserFrequencySelect.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that laser intensity value [0:255].
    /// </summary>
    [DisplayName("LaserIntensityPayload")]
    [Description("Creates a message payload that laser intensity value [0:255].")]
    public partial class CreateLaserIntensityPayload
    {
        /// <summary>
        /// Gets or sets the value that laser intensity value [0:255].
        /// </summary>
        [Description("The value that laser intensity value [0:255].")]
        public byte LaserIntensity { get; set; }

        /// <summary>
        /// Creates a message payload for the LaserIntensity register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return LaserIntensity;
        }

        /// <summary>
        /// Creates a message that laser intensity value [0:255].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LaserIntensity register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.LaserIntensity.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that laser intensity value [0:255].
    /// </summary>
    [DisplayName("TimestampedLaserIntensityPayload")]
    [Description("Creates a timestamped message payload that laser intensity value [0:255].")]
    public partial class CreateTimestampedLaserIntensityPayload : CreateLaserIntensityPayload
    {
        /// <summary>
        /// Creates a timestamped message that laser intensity value [0:255].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LaserIntensity register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.LaserIntensity.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the specified digital output lines.
    /// </summary>
    [DisplayName("OutputSetPayload")]
    [Description("Creates a message payload that set the specified digital output lines.")]
    public partial class CreateOutputSetPayload
    {
        /// <summary>
        /// Gets or sets the value that set the specified digital output lines.
        /// </summary>
        [Description("The value that set the specified digital output lines.")]
        public DigitalOutputs OutputSet { get; set; }

        /// <summary>
        /// Creates a message payload for the OutputSet register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalOutputs GetPayload()
        {
            return OutputSet;
        }

        /// <summary>
        /// Creates a message that set the specified digital output lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OutputSet register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.OutputSet.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the specified digital output lines.
    /// </summary>
    [DisplayName("TimestampedOutputSetPayload")]
    [Description("Creates a timestamped message payload that set the specified digital output lines.")]
    public partial class CreateTimestampedOutputSetPayload : CreateOutputSetPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the specified digital output lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OutputSet register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.OutputSet.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that clear the specified digital output lines.
    /// </summary>
    [DisplayName("OutputClearPayload")]
    [Description("Creates a message payload that clear the specified digital output lines.")]
    public partial class CreateOutputClearPayload
    {
        /// <summary>
        /// Gets or sets the value that clear the specified digital output lines.
        /// </summary>
        [Description("The value that clear the specified digital output lines.")]
        public DigitalOutputs OutputClear { get; set; }

        /// <summary>
        /// Creates a message payload for the OutputClear register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalOutputs GetPayload()
        {
            return OutputClear;
        }

        /// <summary>
        /// Creates a message that clear the specified digital output lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OutputClear register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.OutputClear.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that clear the specified digital output lines.
    /// </summary>
    [DisplayName("TimestampedOutputClearPayload")]
    [Description("Creates a timestamped message payload that clear the specified digital output lines.")]
    public partial class CreateTimestampedOutputClearPayload : CreateOutputClearPayload
    {
        /// <summary>
        /// Creates a timestamped message that clear the specified digital output lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OutputClear register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.OutputClear.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that toggle the specified digital output lines.
    /// </summary>
    [DisplayName("OutputTogglePayload")]
    [Description("Creates a message payload that toggle the specified digital output lines.")]
    public partial class CreateOutputTogglePayload
    {
        /// <summary>
        /// Gets or sets the value that toggle the specified digital output lines.
        /// </summary>
        [Description("The value that toggle the specified digital output lines.")]
        public DigitalOutputs OutputToggle { get; set; }

        /// <summary>
        /// Creates a message payload for the OutputToggle register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalOutputs GetPayload()
        {
            return OutputToggle;
        }

        /// <summary>
        /// Creates a message that toggle the specified digital output lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OutputToggle register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.OutputToggle.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that toggle the specified digital output lines.
    /// </summary>
    [DisplayName("TimestampedOutputTogglePayload")]
    [Description("Creates a timestamped message payload that toggle the specified digital output lines.")]
    public partial class CreateTimestampedOutputTogglePayload : CreateOutputTogglePayload
    {
        /// <summary>
        /// Creates a timestamped message that toggle the specified digital output lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OutputToggle register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.OutputToggle.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that write the state of all digital output lines.
    /// </summary>
    [DisplayName("OutputStatePayload")]
    [Description("Creates a message payload that write the state of all digital output lines.")]
    public partial class CreateOutputStatePayload
    {
        /// <summary>
        /// Gets or sets the value that write the state of all digital output lines.
        /// </summary>
        [Description("The value that write the state of all digital output lines.")]
        public DigitalOutputs OutputState { get; set; }

        /// <summary>
        /// Creates a message payload for the OutputState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalOutputs GetPayload()
        {
            return OutputState;
        }

        /// <summary>
        /// Creates a message that write the state of all digital output lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OutputState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.OutputState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that write the state of all digital output lines.
    /// </summary>
    [DisplayName("TimestampedOutputStatePayload")]
    [Description("Creates a timestamped message payload that write the state of all digital output lines.")]
    public partial class CreateTimestampedOutputStatePayload : CreateOutputStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that write the state of all digital output lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OutputState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.OutputState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configure BNCs to start.
    /// </summary>
    [DisplayName("BncsStatePayload")]
    [Description("Creates a message payload that configure BNCs to start.")]
    public partial class CreateBncsStatePayload
    {
        /// <summary>
        /// Gets or sets the value that configure BNCs to start.
        /// </summary>
        [Description("The value that configure BNCs to start.")]
        public Bncs BncsState { get; set; }

        /// <summary>
        /// Creates a message payload for the BncsState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Bncs GetPayload()
        {
            return BncsState;
        }

        /// <summary>
        /// Creates a message that configure BNCs to start.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the BncsState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.BncsState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configure BNCs to start.
    /// </summary>
    [DisplayName("TimestampedBncsStatePayload")]
    [Description("Creates a timestamped message payload that configure BNCs to start.")]
    public partial class CreateTimestampedBncsStatePayload : CreateBncsStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that configure BNCs to start.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the BncsState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.BncsState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configure Signals to start.
    /// </summary>
    [DisplayName("SignalStatePayload")]
    [Description("Creates a message payload that configure Signals to start.")]
    public partial class CreateSignalStatePayload
    {
        /// <summary>
        /// Gets or sets the value that configure Signals to start.
        /// </summary>
        [Description("The value that configure Signals to start.")]
        public Signals SignalState { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Signals GetPayload()
        {
            return SignalState;
        }

        /// <summary>
        /// Creates a message that configure Signals to start.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configure Signals to start.
    /// </summary>
    [DisplayName("TimestampedSignalStatePayload")]
    [Description("Creates a timestamped message payload that configure Signals to start.")]
    public partial class CreateTimestampedSignalStatePayload : CreateSignalStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that configure Signals to start.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time ON of BNC1 (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("Bnc1OnPayload")]
    [Description("Creates a message payload that time ON of BNC1 (milliseconds) [1:65535].")]
    public partial class CreateBnc1OnPayload
    {
        /// <summary>
        /// Gets or sets the value that time ON of BNC1 (milliseconds) [1:65535].
        /// </summary>
        [Description("The value that time ON of BNC1 (milliseconds) [1:65535].")]
        public ushort Bnc1On { get; set; }

        /// <summary>
        /// Creates a message payload for the Bnc1On register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Bnc1On;
        }

        /// <summary>
        /// Creates a message that time ON of BNC1 (milliseconds) [1:65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Bnc1On register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc1On.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time ON of BNC1 (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("TimestampedBnc1OnPayload")]
    [Description("Creates a timestamped message payload that time ON of BNC1 (milliseconds) [1:65535].")]
    public partial class CreateTimestampedBnc1OnPayload : CreateBnc1OnPayload
    {
        /// <summary>
        /// Creates a timestamped message that time ON of BNC1 (milliseconds) [1:65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Bnc1On register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc1On.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time OFF of BNC1 (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("Bnc1OffPayload")]
    [Description("Creates a message payload that time OFF of BNC1 (milliseconds) [1:65535].")]
    public partial class CreateBnc1OffPayload
    {
        /// <summary>
        /// Gets or sets the value that time OFF of BNC1 (milliseconds) [1:65535].
        /// </summary>
        [Description("The value that time OFF of BNC1 (milliseconds) [1:65535].")]
        public ushort Bnc1Off { get; set; }

        /// <summary>
        /// Creates a message payload for the Bnc1Off register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Bnc1Off;
        }

        /// <summary>
        /// Creates a message that time OFF of BNC1 (milliseconds) [1:65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Bnc1Off register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc1Off.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time OFF of BNC1 (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("TimestampedBnc1OffPayload")]
    [Description("Creates a timestamped message payload that time OFF of BNC1 (milliseconds) [1:65535].")]
    public partial class CreateTimestampedBnc1OffPayload : CreateBnc1OffPayload
    {
        /// <summary>
        /// Creates a timestamped message that time OFF of BNC1 (milliseconds) [1:65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Bnc1Off register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc1Off.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that number of pulses (BNC1) [0;65535], 0-> infinite repeat.
    /// </summary>
    [DisplayName("Bnc1PulsesPayload")]
    [Description("Creates a message payload that number of pulses (BNC1) [0;65535], 0-> infinite repeat.")]
    public partial class CreateBnc1PulsesPayload
    {
        /// <summary>
        /// Gets or sets the value that number of pulses (BNC1) [0;65535], 0-> infinite repeat.
        /// </summary>
        [Description("The value that number of pulses (BNC1) [0;65535], 0-> infinite repeat.")]
        public ushort Bnc1Pulses { get; set; }

        /// <summary>
        /// Creates a message payload for the Bnc1Pulses register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Bnc1Pulses;
        }

        /// <summary>
        /// Creates a message that number of pulses (BNC1) [0;65535], 0-> infinite repeat.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Bnc1Pulses register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc1Pulses.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that number of pulses (BNC1) [0;65535], 0-> infinite repeat.
    /// </summary>
    [DisplayName("TimestampedBnc1PulsesPayload")]
    [Description("Creates a timestamped message payload that number of pulses (BNC1) [0;65535], 0-> infinite repeat.")]
    public partial class CreateTimestampedBnc1PulsesPayload : CreateBnc1PulsesPayload
    {
        /// <summary>
        /// Creates a timestamped message that number of pulses (BNC1) [0;65535], 0-> infinite repeat.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Bnc1Pulses register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc1Pulses.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that wait time to start (milliseconds) (BNC1) [1;65535].
    /// </summary>
    [DisplayName("Bnc1TailPayload")]
    [Description("Creates a message payload that wait time to start (milliseconds) (BNC1) [1;65535].")]
    public partial class CreateBnc1TailPayload
    {
        /// <summary>
        /// Gets or sets the value that wait time to start (milliseconds) (BNC1) [1;65535].
        /// </summary>
        [Description("The value that wait time to start (milliseconds) (BNC1) [1;65535].")]
        public ushort Bnc1Tail { get; set; }

        /// <summary>
        /// Creates a message payload for the Bnc1Tail register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Bnc1Tail;
        }

        /// <summary>
        /// Creates a message that wait time to start (milliseconds) (BNC1) [1;65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Bnc1Tail register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc1Tail.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that wait time to start (milliseconds) (BNC1) [1;65535].
    /// </summary>
    [DisplayName("TimestampedBnc1TailPayload")]
    [Description("Creates a timestamped message payload that wait time to start (milliseconds) (BNC1) [1;65535].")]
    public partial class CreateTimestampedBnc1TailPayload : CreateBnc1TailPayload
    {
        /// <summary>
        /// Creates a timestamped message that wait time to start (milliseconds) (BNC1) [1;65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Bnc1Tail register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc1Tail.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time ON of BNC2 (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("Bnc2OnPayload")]
    [Description("Creates a message payload that time ON of BNC2 (milliseconds) [1:65535].")]
    public partial class CreateBnc2OnPayload
    {
        /// <summary>
        /// Gets or sets the value that time ON of BNC2 (milliseconds) [1:65535].
        /// </summary>
        [Description("The value that time ON of BNC2 (milliseconds) [1:65535].")]
        public ushort Bnc2On { get; set; }

        /// <summary>
        /// Creates a message payload for the Bnc2On register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Bnc2On;
        }

        /// <summary>
        /// Creates a message that time ON of BNC2 (milliseconds) [1:65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Bnc2On register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc2On.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time ON of BNC2 (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("TimestampedBnc2OnPayload")]
    [Description("Creates a timestamped message payload that time ON of BNC2 (milliseconds) [1:65535].")]
    public partial class CreateTimestampedBnc2OnPayload : CreateBnc2OnPayload
    {
        /// <summary>
        /// Creates a timestamped message that time ON of BNC2 (milliseconds) [1:65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Bnc2On register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc2On.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time OFF of BNC2 (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("Bnc2OffPayload")]
    [Description("Creates a message payload that time OFF of BNC2 (milliseconds) [1:65535].")]
    public partial class CreateBnc2OffPayload
    {
        /// <summary>
        /// Gets or sets the value that time OFF of BNC2 (milliseconds) [1:65535].
        /// </summary>
        [Description("The value that time OFF of BNC2 (milliseconds) [1:65535].")]
        public ushort Bnc2Off { get; set; }

        /// <summary>
        /// Creates a message payload for the Bnc2Off register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Bnc2Off;
        }

        /// <summary>
        /// Creates a message that time OFF of BNC2 (milliseconds) [1:65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Bnc2Off register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc2Off.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time OFF of BNC2 (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("TimestampedBnc2OffPayload")]
    [Description("Creates a timestamped message payload that time OFF of BNC2 (milliseconds) [1:65535].")]
    public partial class CreateTimestampedBnc2OffPayload : CreateBnc2OffPayload
    {
        /// <summary>
        /// Creates a timestamped message that time OFF of BNC2 (milliseconds) [1:65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Bnc2Off register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc2Off.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that number of pulses (BNC2) [0;65535], 0-> infinite repeat.
    /// </summary>
    [DisplayName("Bnc2PulsesPayload")]
    [Description("Creates a message payload that number of pulses (BNC2) [0;65535], 0-> infinite repeat.")]
    public partial class CreateBnc2PulsesPayload
    {
        /// <summary>
        /// Gets or sets the value that number of pulses (BNC2) [0;65535], 0-> infinite repeat.
        /// </summary>
        [Description("The value that number of pulses (BNC2) [0;65535], 0-> infinite repeat.")]
        public ushort Bnc2Pulses { get; set; }

        /// <summary>
        /// Creates a message payload for the Bnc2Pulses register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Bnc2Pulses;
        }

        /// <summary>
        /// Creates a message that number of pulses (BNC2) [0;65535], 0-> infinite repeat.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Bnc2Pulses register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc2Pulses.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that number of pulses (BNC2) [0;65535], 0-> infinite repeat.
    /// </summary>
    [DisplayName("TimestampedBnc2PulsesPayload")]
    [Description("Creates a timestamped message payload that number of pulses (BNC2) [0;65535], 0-> infinite repeat.")]
    public partial class CreateTimestampedBnc2PulsesPayload : CreateBnc2PulsesPayload
    {
        /// <summary>
        /// Creates a timestamped message that number of pulses (BNC2) [0;65535], 0-> infinite repeat.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Bnc2Pulses register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc2Pulses.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that wait time to start (milliseconds) (BNC2) [1;65535].
    /// </summary>
    [DisplayName("Bnc2TailPayload")]
    [Description("Creates a message payload that wait time to start (milliseconds) (BNC2) [1;65535].")]
    public partial class CreateBnc2TailPayload
    {
        /// <summary>
        /// Gets or sets the value that wait time to start (milliseconds) (BNC2) [1;65535].
        /// </summary>
        [Description("The value that wait time to start (milliseconds) (BNC2) [1;65535].")]
        public ushort Bnc2Tail { get; set; }

        /// <summary>
        /// Creates a message payload for the Bnc2Tail register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Bnc2Tail;
        }

        /// <summary>
        /// Creates a message that wait time to start (milliseconds) (BNC2) [1;65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Bnc2Tail register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc2Tail.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that wait time to start (milliseconds) (BNC2) [1;65535].
    /// </summary>
    [DisplayName("TimestampedBnc2TailPayload")]
    [Description("Creates a timestamped message payload that wait time to start (milliseconds) (BNC2) [1;65535].")]
    public partial class CreateTimestampedBnc2TailPayload : CreateBnc2TailPayload
    {
        /// <summary>
        /// Creates a timestamped message that wait time to start (milliseconds) (BNC2) [1;65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Bnc2Tail register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.Bnc2Tail.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time ON of SignalA (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("SignalAOnPayload")]
    [Description("Creates a message payload that time ON of SignalA (milliseconds) [1:65535].")]
    public partial class CreateSignalAOnPayload
    {
        /// <summary>
        /// Gets or sets the value that time ON of SignalA (milliseconds) [1:65535].
        /// </summary>
        [Description("The value that time ON of SignalA (milliseconds) [1:65535].")]
        public ushort SignalAOn { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalAOn register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SignalAOn;
        }

        /// <summary>
        /// Creates a message that time ON of SignalA (milliseconds) [1:65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalAOn register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalAOn.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time ON of SignalA (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("TimestampedSignalAOnPayload")]
    [Description("Creates a timestamped message payload that time ON of SignalA (milliseconds) [1:65535].")]
    public partial class CreateTimestampedSignalAOnPayload : CreateSignalAOnPayload
    {
        /// <summary>
        /// Creates a timestamped message that time ON of SignalA (milliseconds) [1:65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalAOn register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalAOn.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time OFF of SignalA (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("SignalAOffPayload")]
    [Description("Creates a message payload that time OFF of SignalA (milliseconds) [1:65535].")]
    public partial class CreateSignalAOffPayload
    {
        /// <summary>
        /// Gets or sets the value that time OFF of SignalA (milliseconds) [1:65535].
        /// </summary>
        [Description("The value that time OFF of SignalA (milliseconds) [1:65535].")]
        public ushort SignalAOff { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalAOff register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SignalAOff;
        }

        /// <summary>
        /// Creates a message that time OFF of SignalA (milliseconds) [1:65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalAOff register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalAOff.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time OFF of SignalA (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("TimestampedSignalAOffPayload")]
    [Description("Creates a timestamped message payload that time OFF of SignalA (milliseconds) [1:65535].")]
    public partial class CreateTimestampedSignalAOffPayload : CreateSignalAOffPayload
    {
        /// <summary>
        /// Creates a timestamped message that time OFF of SignalA (milliseconds) [1:65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalAOff register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalAOff.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that number of pulses (SignalA) [0;65535], 0-> infinite repeat.
    /// </summary>
    [DisplayName("SignalAPulsesPayload")]
    [Description("Creates a message payload that number of pulses (SignalA) [0;65535], 0-> infinite repeat.")]
    public partial class CreateSignalAPulsesPayload
    {
        /// <summary>
        /// Gets or sets the value that number of pulses (SignalA) [0;65535], 0-> infinite repeat.
        /// </summary>
        [Description("The value that number of pulses (SignalA) [0;65535], 0-> infinite repeat.")]
        public ushort SignalAPulses { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalAPulses register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SignalAPulses;
        }

        /// <summary>
        /// Creates a message that number of pulses (SignalA) [0;65535], 0-> infinite repeat.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalAPulses register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalAPulses.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that number of pulses (SignalA) [0;65535], 0-> infinite repeat.
    /// </summary>
    [DisplayName("TimestampedSignalAPulsesPayload")]
    [Description("Creates a timestamped message payload that number of pulses (SignalA) [0;65535], 0-> infinite repeat.")]
    public partial class CreateTimestampedSignalAPulsesPayload : CreateSignalAPulsesPayload
    {
        /// <summary>
        /// Creates a timestamped message that number of pulses (SignalA) [0;65535], 0-> infinite repeat.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalAPulses register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalAPulses.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that wait time to start (milliseconds) (SignalA) [1;65535].
    /// </summary>
    [DisplayName("SignalATailPayload")]
    [Description("Creates a message payload that wait time to start (milliseconds) (SignalA) [1;65535].")]
    public partial class CreateSignalATailPayload
    {
        /// <summary>
        /// Gets or sets the value that wait time to start (milliseconds) (SignalA) [1;65535].
        /// </summary>
        [Description("The value that wait time to start (milliseconds) (SignalA) [1;65535].")]
        public ushort SignalATail { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalATail register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SignalATail;
        }

        /// <summary>
        /// Creates a message that wait time to start (milliseconds) (SignalA) [1;65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalATail register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalATail.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that wait time to start (milliseconds) (SignalA) [1;65535].
    /// </summary>
    [DisplayName("TimestampedSignalATailPayload")]
    [Description("Creates a timestamped message payload that wait time to start (milliseconds) (SignalA) [1;65535].")]
    public partial class CreateTimestampedSignalATailPayload : CreateSignalATailPayload
    {
        /// <summary>
        /// Creates a timestamped message that wait time to start (milliseconds) (SignalA) [1;65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalATail register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalATail.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time ON of SignalB (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("SignalBOnPayload")]
    [Description("Creates a message payload that time ON of SignalB (milliseconds) [1:65535].")]
    public partial class CreateSignalBOnPayload
    {
        /// <summary>
        /// Gets or sets the value that time ON of SignalB (milliseconds) [1:65535].
        /// </summary>
        [Description("The value that time ON of SignalB (milliseconds) [1:65535].")]
        public ushort SignalBOn { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalBOn register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SignalBOn;
        }

        /// <summary>
        /// Creates a message that time ON of SignalB (milliseconds) [1:65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalBOn register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalBOn.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time ON of SignalB (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("TimestampedSignalBOnPayload")]
    [Description("Creates a timestamped message payload that time ON of SignalB (milliseconds) [1:65535].")]
    public partial class CreateTimestampedSignalBOnPayload : CreateSignalBOnPayload
    {
        /// <summary>
        /// Creates a timestamped message that time ON of SignalB (milliseconds) [1:65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalBOn register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalBOn.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that time OFF of SignalB (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("SignalBOffPayload")]
    [Description("Creates a message payload that time OFF of SignalB (milliseconds) [1:65535].")]
    public partial class CreateSignalBOffPayload
    {
        /// <summary>
        /// Gets or sets the value that time OFF of SignalB (milliseconds) [1:65535].
        /// </summary>
        [Description("The value that time OFF of SignalB (milliseconds) [1:65535].")]
        public ushort SignalBOff { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalBOff register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SignalBOff;
        }

        /// <summary>
        /// Creates a message that time OFF of SignalB (milliseconds) [1:65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalBOff register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalBOff.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that time OFF of SignalB (milliseconds) [1:65535].
    /// </summary>
    [DisplayName("TimestampedSignalBOffPayload")]
    [Description("Creates a timestamped message payload that time OFF of SignalB (milliseconds) [1:65535].")]
    public partial class CreateTimestampedSignalBOffPayload : CreateSignalBOffPayload
    {
        /// <summary>
        /// Creates a timestamped message that time OFF of SignalB (milliseconds) [1:65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalBOff register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalBOff.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that number of pulses (SignalB) [0;65535], 0-> infinite repeat.
    /// </summary>
    [DisplayName("SignalBPulsesPayload")]
    [Description("Creates a message payload that number of pulses (SignalB) [0;65535], 0-> infinite repeat.")]
    public partial class CreateSignalBPulsesPayload
    {
        /// <summary>
        /// Gets or sets the value that number of pulses (SignalB) [0;65535], 0-> infinite repeat.
        /// </summary>
        [Description("The value that number of pulses (SignalB) [0;65535], 0-> infinite repeat.")]
        public ushort SignalBPulses { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalBPulses register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SignalBPulses;
        }

        /// <summary>
        /// Creates a message that number of pulses (SignalB) [0;65535], 0-> infinite repeat.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalBPulses register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalBPulses.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that number of pulses (SignalB) [0;65535], 0-> infinite repeat.
    /// </summary>
    [DisplayName("TimestampedSignalBPulsesPayload")]
    [Description("Creates a timestamped message payload that number of pulses (SignalB) [0;65535], 0-> infinite repeat.")]
    public partial class CreateTimestampedSignalBPulsesPayload : CreateSignalBPulsesPayload
    {
        /// <summary>
        /// Creates a timestamped message that number of pulses (SignalB) [0;65535], 0-> infinite repeat.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalBPulses register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalBPulses.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that wait time to start (milliseconds) (SignalB) [1;65535].
    /// </summary>
    [DisplayName("SignalBTailPayload")]
    [Description("Creates a message payload that wait time to start (milliseconds) (SignalB) [1;65535].")]
    public partial class CreateSignalBTailPayload
    {
        /// <summary>
        /// Gets or sets the value that wait time to start (milliseconds) (SignalB) [1;65535].
        /// </summary>
        [Description("The value that wait time to start (milliseconds) (SignalB) [1;65535].")]
        public ushort SignalBTail { get; set; }

        /// <summary>
        /// Creates a message payload for the SignalBTail register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SignalBTail;
        }

        /// <summary>
        /// Creates a message that wait time to start (milliseconds) (SignalB) [1;65535].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SignalBTail register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.SignalBTail.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that wait time to start (milliseconds) (SignalB) [1;65535].
    /// </summary>
    [DisplayName("TimestampedSignalBTailPayload")]
    [Description("Creates a timestamped message payload that wait time to start (milliseconds) (SignalB) [1;65535].")]
    public partial class CreateTimestampedSignalBTailPayload : CreateSignalBTailPayload
    {
        /// <summary>
        /// Creates a timestamped message that wait time to start (milliseconds) (SignalB) [1;65535].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SignalBTail register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.SignalBTail.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the active events in the device.
    /// </summary>
    [DisplayName("EventEnablePayload")]
    [Description("Creates a message payload that specifies the active events in the device.")]
    public partial class CreateEventEnablePayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the active events in the device.
        /// </summary>
        [Description("The value that specifies the active events in the device.")]
        public LaserDriverControllerEvents EventEnable { get; set; }

        /// <summary>
        /// Creates a message payload for the EventEnable register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LaserDriverControllerEvents GetPayload()
        {
            return EventEnable;
        }

        /// <summary>
        /// Creates a message that specifies the active events in the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EventEnable register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.LaserDriverController.EventEnable.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the active events in the device.
    /// </summary>
    [DisplayName("TimestampedEventEnablePayload")]
    [Description("Creates a timestamped message payload that specifies the active events in the device.")]
    public partial class CreateTimestampedEventEnablePayload : CreateEventEnablePayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the active events in the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EventEnable register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.LaserDriverController.EventEnable.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Specifies the state of port digital output lines.
    /// </summary>
    [Flags]
    public enum DigitalOutputs : byte
    {
        None = 0x0,
        DO1 = 0x1,
        DO2 = 0x2
    }

    /// <summary>
    /// Specifies the state of BNCs
    /// </summary>
    [Flags]
    public enum Bncs : byte
    {
        None = 0x0,
        Bnc1 = 0x1,
        Bnc2 = 0x2
    }

    /// <summary>
    /// Specifies the state of Signals
    /// </summary>
    [Flags]
    public enum Signals : byte
    {
        None = 0x0,
        SignalA = 0x1,
        SignalB = 0x2
    }

    /// <summary>
    /// Specifies the active events in the device
    /// </summary>
    [Flags]
    public enum LaserDriverControllerEvents : byte
    {
        None = 0x0,
        EventSpadSwitch = 0x1,
        EventLaserState = 0x2
    }

    /// <summary>
    /// Selects laser frequency mode
    /// </summary>
    public enum FrequencySelect : byte
    {
        None = 0,
        F1 = 1,
        F2 = 2,
        F3 = 4,
        CW = 8
    }
}
