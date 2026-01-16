using System;
using System.Collections.Generic;
using System.Linq;
using Conversion;
using DataReceiving;
using Serialization;

namespace ExportDataService;

/// <summary>
/// Generic export data service.
/// </summary>
/// <typeparam name="T">Type to export.</typeparam>
public class ExportDataService<T>
{
    private readonly IDataReceiver receiver;
    private readonly IDataSerializer<T> serializer;
    private readonly IConverter<T> converter;

    public ExportDataService(IDataReceiver receiver, IDataSerializer<T> serializer, IConverter<T> converter)
    {
        this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    public void Run()
    {
        IEnumerable<string> lines = this.receiver.Receive() ?? Enumerable.Empty<string>();

        // Важно: конвертируем каждую строку (Count раз в тестах)
        List<T> result = new ();

        foreach (string line in lines)
        {
            T? converted = this.converter.Convert(line);
            if (converted is not null)
            {
                result.Add(converted);
            }
        }

        // Serialize должен вызваться ровно один раз
        this.serializer.Serialize(result);
    }
}
