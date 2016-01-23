namespace MSHealthAPI
{
    using Newtonsoft.Json;
    using System;
    using System.Xml;

    /// <summary>
    /// Converts from/to <see cref="System.TimeSpan"/> to/from Json value.
    /// </summary>
    public sealed class TimeSpanConverter : JsonConverter
    {

        #region Methods Overriding

        /// <summary>
        /// Writes the JSON representation of the <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="TimeSpan"/> value.</param>
        /// <param name="serializer">The calling <see cref="JsonSerializer"/>.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TimeSpan loTimeSpan = (TimeSpan)value;
            string lsTimeSpanString = XmlConvert.ToString(loTimeSpan);
            serializer.Serialize(writer, lsTimeSpanString);
        }

        /// <summary>
        /// Reads the JSON representation of the <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader/> to read from.</param>
        /// <param name="objectType">Type of the object (<see cref="TimeSpan"/>).</param>
        /// <param name="existingValue">The existing value of <see cref="TimeSpan"/> being read.</param>
        /// <param name="serializer">The calling <see cref="JsonSerializer"/>.</param>
        /// <returns>The <see cref="TimeSpan"/> value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            string lsValue = serializer.Deserialize<string>(reader);
            return XmlConvert.ToTimeSpan(lsValue);
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><see langword="true"/> if this instance can convert the specified object type; otherwise, <see langword="false"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan) || objectType == typeof(TimeSpan?);
        }

        #endregion

    }

}
