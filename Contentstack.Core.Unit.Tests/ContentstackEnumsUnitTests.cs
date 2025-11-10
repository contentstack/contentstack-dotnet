using System;
using System.Linq;
using AutoFixture;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for ContentstackEnums (CachePolicy, OrderBy, ResponseType, NetworkStatus) - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class ContentstackEnumsUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        #region CachePolicy Tests

        [Fact]
        public void CachePolicy_NetworkOnly_HasCorrectValue()
        {
            // Act
            var policy = CachePolicy.NetworkOnly;

            // Assert
            Assert.Equal(0, (int)policy);
        }

        [Fact]
        public void CachePolicy_Parse_WithValidString_ReturnsCorrectValue()
        {
            // Act
            var result = Enum.Parse<CachePolicy>("NetworkOnly");

            // Assert
            Assert.Equal(CachePolicy.NetworkOnly, result);
        }

        [Fact]
        public void CachePolicy_GetValues_ReturnsAllPolicies()
        {
            // Act
            var values = Enum.GetValues(typeof(CachePolicy));

            // Assert
            Assert.NotNull(values);
            Assert.Single(values);
            var policyArray = values.Cast<CachePolicy>().ToArray();
            Assert.Contains(CachePolicy.NetworkOnly, policyArray);
        }

        #endregion

        #region OrderBy Tests

        [Fact]
        public void OrderBy_OrderByAscending_HasCorrectValue()
        {
            // Act
            var orderBy = OrderBy.OrderByAscending;

            // Assert
            Assert.Equal(0, (int)orderBy);
        }

        [Fact]
        public void OrderBy_OrderByDescending_HasCorrectValue()
        {
            // Act
            var orderBy = OrderBy.OrderByDescending;

            // Assert
            Assert.Equal(1, (int)orderBy);
        }

        [Fact]
        public void OrderBy_Parse_WithValidString_ReturnsCorrectValue()
        {
            // Act
            var result = Enum.Parse<OrderBy>("OrderByAscending");

            // Assert
            Assert.Equal(OrderBy.OrderByAscending, result);
        }

        [Fact]
        public void OrderBy_GetValues_ReturnsAllValues()
        {
            // Act
            var values = Enum.GetValues(typeof(OrderBy));

            // Assert
            Assert.NotNull(values);
            Assert.Equal(2, values.Length);
            var orderByArray = values.Cast<OrderBy>().ToArray();
            Assert.Contains(OrderBy.OrderByAscending, orderByArray);
            Assert.Contains(OrderBy.OrderByDescending, orderByArray);
        }

        #endregion

        #region ResponseType Tests

        [Fact]
        public void ResponseType_Cache_HasCorrectValue()
        {
            // Act
            var responseType = ResponseType.Cache;

            // Assert
            Assert.Equal(0, (int)responseType);
        }

        [Fact]
        public void ResponseType_Network_HasCorrectValue()
        {
            // Act
            var responseType = ResponseType.Network;

            // Assert
            Assert.Equal(1, (int)responseType);
        }

        [Fact]
        public void ResponseType_Unknown_HasCorrectValue()
        {
            // Act
            var responseType = ResponseType.Unknown;

            // Assert
            Assert.Equal(2, (int)responseType);
        }

        [Fact]
        public void ResponseType_Parse_WithValidString_ReturnsCorrectValue()
        {
            // Act
            var result = Enum.Parse<ResponseType>("Cache");

            // Assert
            Assert.Equal(ResponseType.Cache, result);
        }

        [Fact]
        public void ResponseType_GetValues_ReturnsAllValues()
        {
            // Act
            var values = Enum.GetValues(typeof(ResponseType));

            // Assert
            Assert.NotNull(values);
            Assert.Equal(3, values.Length);
            var responseTypeArray = values.Cast<ResponseType>().ToArray();
            Assert.Contains(ResponseType.Cache, responseTypeArray);
            Assert.Contains(ResponseType.Network, responseTypeArray);
            Assert.Contains(ResponseType.Unknown, responseTypeArray);
        }

        #endregion

        #region NetworkStatus Tests

        [Fact]
        public void NetworkStatus_NotReachable_HasCorrectValue()
        {
            // Act
            var status = NetworkStatus.NotReachable;

            // Assert
            Assert.Equal(0, (int)status);
        }

        [Fact]
        public void NetworkStatus_ReachableViaCarrierDataNetwork_HasCorrectValue()
        {
            // Act
            var status = NetworkStatus.ReachableViaCarrierDataNetwork;

            // Assert
            Assert.Equal(1, (int)status);
        }

        [Fact]
        public void NetworkStatus_ReachableViaWiFiNetwork_HasCorrectValue()
        {
            // Act
            var status = NetworkStatus.ReachableViaWiFiNetwork;

            // Assert
            Assert.Equal(2, (int)status);
        }

        [Fact]
        public void NetworkStatus_Parse_WithValidString_ReturnsCorrectValue()
        {
            // Act
            var result = Enum.Parse<NetworkStatus>("NotReachable");

            // Assert
            Assert.Equal(NetworkStatus.NotReachable, result);
        }

        [Fact]
        public void NetworkStatus_GetValues_ReturnsAllValues()
        {
            // Act
            var values = Enum.GetValues(typeof(NetworkStatus));

            // Assert
            Assert.NotNull(values);
            Assert.Equal(3, values.Length);
            var networkStatusArray = values.Cast<NetworkStatus>().ToArray();
            Assert.Contains(NetworkStatus.NotReachable, networkStatusArray);
            Assert.Contains(NetworkStatus.ReachableViaCarrierDataNetwork, networkStatusArray);
            Assert.Contains(NetworkStatus.ReachableViaWiFiNetwork, networkStatusArray);
        }

        #endregion
    }
}

