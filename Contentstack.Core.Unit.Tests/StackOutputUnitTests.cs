using System;
using System.Reflection;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    public class StackOutputUnitTests
    {
        [Fact]
        public void StackOutput_DefaultConstructor_InitializesFields()
        {
            // Arrange & Act
            var stackOutput = new StackOutput();

            // Assert - Use reflection to verify private field initialization
            var type = typeof(StackOutput);
            var totalCountField = type.GetField("_TotalCount", BindingFlags.NonPublic | BindingFlags.Instance);
            var jsonField = type.GetField("_Json", BindingFlags.NonPublic | BindingFlags.Instance);
            var noticeField = type.GetField("_Notice", BindingFlags.NonPublic | BindingFlags.Instance);
            var ownerField = type.GetField("_Owner", BindingFlags.NonPublic | BindingFlags.Instance);
            var uidField = type.GetField("_Uid", BindingFlags.NonPublic | BindingFlags.Instance);
            var objectAttributesField = type.GetField("_ObjectAttributes", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(totalCountField);
            Assert.NotNull(jsonField);
            Assert.NotNull(noticeField);
            Assert.NotNull(ownerField);
            Assert.NotNull(uidField);
            Assert.NotNull(objectAttributesField);

            var totalCount = totalCountField.GetValue(stackOutput);
            var json = jsonField.GetValue(stackOutput);
            var notice = noticeField.GetValue(stackOutput);
            var owner = ownerField.GetValue(stackOutput);
            var uid = uidField.GetValue(stackOutput);
            var objectAttributes = objectAttributesField.GetValue(stackOutput);

            Assert.Equal(0, totalCount);
            Assert.Equal(string.Empty, json);
            Assert.Equal(string.Empty, notice);
            Assert.Equal(string.Empty, owner);
            Assert.Equal(string.Empty, uid);
            Assert.NotNull(objectAttributes);
        }

        [Fact]
        public void StackOutput_PrivateFields_CanBeSet()
        {
            // Arrange
            var stackOutput = new StackOutput();
            var type = typeof(StackOutput);

            // Act - Set private fields using reflection
            var totalCountField = type.GetField("_TotalCount", BindingFlags.NonPublic | BindingFlags.Instance);
            var jsonField = type.GetField("_Json", BindingFlags.NonPublic | BindingFlags.Instance);
            var noticeField = type.GetField("_Notice", BindingFlags.NonPublic | BindingFlags.Instance);
            var ownerField = type.GetField("_Owner", BindingFlags.NonPublic | BindingFlags.Instance);
            var uidField = type.GetField("_Uid", BindingFlags.NonPublic | BindingFlags.Instance);

            totalCountField.SetValue(stackOutput, 42);
            jsonField.SetValue(stackOutput, "test json");
            noticeField.SetValue(stackOutput, "test notice");
            ownerField.SetValue(stackOutput, "test owner");
            uidField.SetValue(stackOutput, "test uid");

            // Assert
            Assert.Equal(42, totalCountField.GetValue(stackOutput));
            Assert.Equal("test json", jsonField.GetValue(stackOutput));
            Assert.Equal("test notice", noticeField.GetValue(stackOutput));
            Assert.Equal("test owner", ownerField.GetValue(stackOutput));
            Assert.Equal("test uid", uidField.GetValue(stackOutput));
        }

        [Fact]
        public void StackOutput_DefaultObjectFields_AreNull()
        {
            // Arrange
            var stackOutput = new StackOutput();
            var type = typeof(StackOutput);

            // Act - Get private fields using reflection
            var outputField = type.GetField("_Output", BindingFlags.NonPublic | BindingFlags.Instance);
            var schemaField = type.GetField("_Schema", BindingFlags.NonPublic | BindingFlags.Instance);
            var objectField = type.GetField("_Object", BindingFlags.NonPublic | BindingFlags.Instance);
            var objectsField = type.GetField("_Objects", BindingFlags.NonPublic | BindingFlags.Instance);
            var resultField = type.GetField("_Result", BindingFlags.NonPublic | BindingFlags.Instance);
            var applicationUserField = type.GetField("_ApplicationUser", BindingFlags.NonPublic | BindingFlags.Instance);
            var tagsField = type.GetField("_Tags", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.Equal(default(object), outputField.GetValue(stackOutput));
            Assert.Equal(default(object), schemaField.GetValue(stackOutput));
            Assert.Equal(default(object), objectField.GetValue(stackOutput));
            Assert.Equal(default(object), objectsField.GetValue(stackOutput));
            Assert.Equal(default(object), resultField.GetValue(stackOutput));
            Assert.Equal(default(object), applicationUserField.GetValue(stackOutput));
            Assert.Equal(default(object), tagsField.GetValue(stackOutput));
        }
    }
}



