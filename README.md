# BaseSerializer

## What is this?

BaseSerializer is an XML based serializer capable of complex(e.g. cyclic) data structures. This is achieved by internally treating everything a reference and splitting object creation into two steps: Creation and actual deserilization.

## How to use?

Install this library like you would any other. This library has a simple public API.

### Serialization

```csharp
BaseSerializer serializer = new BaseSerializer();
XDocument xmlDocument = serializer.Serialize(myObject);
```

You can then manually transform this document or save it to the disc. ([XDocument documentation](https://docs.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument?view=netcore-2.1))

```csharp
xmlDocument.Save("~/Documents/Data.xml");
```

### Deserialization

A XDocument reference can be deserialized to an object.

```csharp
xmlDocument = XDocument.Load("~/Documents/Data.xml");
myObject = serializer.Deserialize(xmlDocument);
```

You can also optionally deserialize into an already existing object(primarily useful for usage in Unity3d where you do not have the ability to manually create some object instances).

```csharp
xmlDocument = XDocument.Load("~/Documents/Data.xml");
serializer.Deserialize(xmlDocument, myObject);
```

### Extension

The base serializer aims to provide a good amount of extensibility in the form of custom serializer implementations.

#### IBaseSerializer

All serializers must implement the `IBaseSerializer` interface which contains various properties and methods required for serialization and deserialization.

##### Order

```csharp
int Order { get; }
```

The order of this serializer. Serializers with a higher order will be considered first. This can be useful if several serializers would clash with each other.

##### IsHandled

```csharp
bool IsHandled(SerializationContext.Ref reference);
```

Checks if the given type can be handled by this serializer. If this method returns true we indicate that this serializer is willing and ready to deserialize the given reference.

The `Ref` type is a commonly used container type to safely dereference objects in the base inspector. See below for documentation on its public API.

##### Serialize

```csharp
void Serialize(SerializationContext.Ref target, SerializationContext context);
```

Serializes the given value. The amount of parameters might look confusing, but the `Ref` contains both the object that should be serialized aswell as the XML element it should be serialized into.

The `SerializationContext` holds data about the ongoing serialization process and the `BaseSerializer` instance contains methods to serialize child values. See below for documentation on its public API.

##### CreateInstance

```csharp
object CreateInstance(SerializationContext.Ref source);
```

Creates an instance from deserialized data. The instance must not be populated right away, but an instance must be created and used later on.

This method is essential to the base inspectors special handling of references. Try to do as little work here as possible, just the bare minimum necessary to construct an object.

##### Deserialize

```csharp
void Deserialize(SerializationContext.Ref source, SerializationContext context);
```

This method performans the actual deserilization. You can go wild with your deserilization logic here. The object previously created by `CreateInstance` can be found inside the `Ref`.

This method is not guaranteed to immediately run after `CreateInstance` and execution could be stalled in case of cyclic references until the hierarchy has been unwinded.

#### Ref

The `Ref`(or `SerializationContext.Ref`) type is used to hold and safely dereference objects within the base serializer and contains some metadata in addition to the object itself.

##### Type

```csharp
public Type Type { get; }
```

The type of the object. If the objet is `null` this property is also `null`.

##### Serializer

```csharp
public BaseSerializer Serializer { get; }
```

The serializer associated with this reference. If the object is `null` this property may also be `null`. It is recommended to instead rely on the `Serializer` property in the context, which is never `null`.

##### Id

```csharp
public string Id { get; }
```

Every reference has a unique ID. This property can be used to obtain the ID of the reference. This is what you typically want to store if you implement a custom serializer that has child values it needs to serialize.

##### Object

```csharp
public object Object { get; }
```

The actual object associated with this reference.

##### Element

```csharp
public XElement Element { get; }
```

The XML element that represents this reference. Use this to write all data to that is required from deserialization later.

#### SerializationContext

A `SerializationContext` is used during a single serilization or deserialization execution and holds data relevant to this process.

##### Serializer

```csharp
public BaseSerializer Serializer { get; }
```

The serializer associated with this context.

#### BaseSerializer

##### Serializers

```csharp
public IList<IBaseSerializer> Serializers { get; }
```

The base serializer delegates the handling of individual data types to different serializer implementations. You can get/add/change all implementations use by a base serializer though its `Serializers` property.

Contains `FallbackSerializer.Instance`, `IntegerSerializer.Instance`,  `StringSerializer.Instance` & `FloatSerializer.Instance` by default.

##### AssemblyAliases

```csharp
public IDictionary<string, Assembly> AssemblyAliases { get; }
```

Since the base serializer also saves the type these aliases can be used to shorten the type name. The default names are rather long(`/YourApp.NameSpace.YourClass+ChildClass, App, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null`), but with an alias it can be shortened significantly(`app/YourApp.NameSpace.YourClass+ChildClass` - in this case, `app` was used as alias for the `App` assembly.).

Contains `mscorlib` as an alias for the .NET standard library and `serializer` for the base serializer assembly by default.

##### Serialize

```csharp
public XDocument Serialize(object data)
```

This method serializes the given object as a XML document.

##### Deserialize

```csharp
public object Deserialize(XDocument document)
public void Deserialize(XDocument document, object target)
```

Deserializes either the given XML document into a new object(first variant), or into an already existing object(primarily useful for usage in Unity3d where you do not have the ability to manually create some object instances).

##### SerializeReference

```csharp
public SerializationContext.Ref SerializeReference(object data, SerializationContext ctx)
```

Serializes the given object into a `Ref`. If the object has been serialized before its `Ref` will be reused. This method is meant to be used inside of custom serializers. See examples below for further details.

##### DeserializeReference

```csharp
public SerializationContext.Ref DeserializeReference(string id, SerializationContext ctx)
```

This method deserializes an object. Pass the ID of a reference and it will return the fully deserialized `Ref`(or null if the ID does not exist). See examples below for further details.

### Examples

All examples mentioned here can be found(with some minor modifications) in `Test/Examples` directory. In genereal the `Test` directiory is a good place to look if you are unsure on how to use the public API of the base serializer.

#### Simple Serialization

The code can be found here: `Test/Examples/SimpleSerialization.cs` 

First of all we'll create a simple class to serialize:

```csharp
public class UserAccount
{
    public string Name;
    public string Gender;
    public int Age;

    public UserAccount(string name, string gender, int age)
    {
        Name = name;
        Gender = gender;
        Age = age;
    }
}
```

By default the base serializer only serializes fields, so we'll just use simple public fields for everything. If you want to serialize properties there is an example in the `Test/Examples` directory for that aswell.

Now let's get to writing the actual save method. 

```csharp
public void SaveData()
{
    UserAccount account = new UserAccount("Mike Johnson", "Male", 36);

    BaseSerializer serializer = new BaseSerializer();
    XDocument document = serializer.Serialize(account);
    document.Save("~/Documents/Data.xml");
}
```

This will create the following `Data.xml` file:

```xml
<Data root="ref-1">
  <ref-1 type="/Test.Examples.SimpleSerialization+UserAccount, Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
    <Name>ref-2</Name>
    <Gender>ref-3</Gender>
    <Age>ref-4</Age>
  </ref-1>
  <ref-2 type="mscorlib/System.String">Mike Johnson</ref-2>
  <ref-3 type="mscorlib/System.String">Male</ref-3>
  <ref-4 type="mscorlib/System.Int32">36</ref-4>
</Data>
```

Every field has been serialized into its own reference.

#### Cyclic Serialization

The code can be found here: `Test/Examples/CyclicSerialization.cs` 

However not all values are typically simple primitives. In a language such as C# we heavily work with classes having references to each other, which has a possibility to introduce cyclic references.

To demonstrate this let's add another field to the `UserAccount` class: A list of friends.

```csharp
public IList<UserAccount> Friends = new List<UserAccount>();
```

Now let's create some cyclic user accounts.

```csharp
public void SaveData()
{
    UserAccount account = new UserAccount("Mike Johnson", "Male", 36);
    UserAccount jane = new UserAccount("Jane Doe", "Female", 42);
    UserAccount john = new UserAccount("John Harper", "Male", 57);
    account.Friends.Add(jane);
    account.Friends.Add(john);
    jane.Friends.Add(john);
    john.Friends.Add(jane);
    
    BaseSerializer serializer = new BaseSerializer();
    XDocument document = serializer.Serialize(account);
    document.Save("~/Documents/Data.xml");
}
```

And the output is(behold!):

```xml
<Data root="ref-1">
  <ref-1 type="/Test.Examples.CyclicSerialization+UserAccount, Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
    <Name>ref-2</Name>
    <Gender>ref-3</Gender>
    <Age>ref-4</Age>
    <Friends>ref-5</Friends>
  </ref-1>
  <ref-2 type="mscorlib/System.String">Mike Johnson</ref-2>
  <ref-3 type="mscorlib/System.String">Male</ref-3>
  <ref-4 type="mscorlib/System.Int32">36</ref-4>
  <ref-5 type="mscorlib/System.Collections.Generic.List`1[[Test.Examples.CyclicSerialization+UserAccount, Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]">
    <Entry>ref-6</Entry>
  </ref-5>
  <ref-6 type="/Test.Examples.CyclicSerialization+UserAccount, Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
    <Name>ref-7</Name>
    <Gender>ref-3</Gender>
    <Age>ref-8</Age>
    <Friends>ref-9</Friends>
  </ref-6>
  <ref-7 type="mscorlib/System.String">John Harper</ref-7>
  <ref-8 type="mscorlib/System.Int32">57</ref-8>
  <ref-9 type="mscorlib/System.Collections.Generic.List`1[[Test.Examples.CyclicSerialization+UserAccount, Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]">
    <Entry>ref-1</Entry>
  </ref-9>
</Data>
```

Let's take a look at this structure: Our root `<Data>` node has an attribute called `root`. This where our object begins. In our case this is `ref-1`, which is just a line below.

Its `<Friends>` node references our list of friends(`account.Friends`) at `ref-5`. This list has an entry for John(`ref-6`) which in turn has a friends list(`ref-9`) that contains an entry back to `ref-1`.

Furthermore you may have noticed that the `<Gender>` of both Mike and John uses `ref-3`. Since everything is handled using the reference system internally no value will be serialized twice.
