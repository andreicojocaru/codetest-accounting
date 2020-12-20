using System.Threading.Tasks;
using CodeTest.Accounting.Persistence.Tests.Fixtures;
using NUnit.Framework;

namespace CodeTest.Accounting.Persistence.Tests
{
    public class InMemoryRepositoryTests
    {
        private InMemoryRepository<MockEntity> _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new InMemoryRepository<MockEntity>();
        }

        [Test]
        public async Task GetAsync_NoEntity_ShouldReturnDefault()
        {
            // Arrange
            var id = 1;
            MockEntity expected = null;

            // Act
            var actual = await _sut.GetAsync(id);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GetAsync_WithEntity_ShouldReturnEntity()
        {
            // Arrange
            var id = 1;
            var expected = new MockEntity
            {
                Id = id
            };

            await _sut.SetAsync(expected);

            // Act
            var actual = await _sut.GetAsync(id);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Id, actual.Id);
        }

        [Test]
        public async Task SetAsync_ShouldReturnEntityId()
        {
            // Arrange
            var id = 1;
            var entity = new MockEntity
            {
                Id = id
            };


            // Act
            var actual = await _sut.SetAsync(entity);

            // Assert
            Assert.AreEqual(id, actual);
        }

        [Test]
        public async Task SetAsync_MultipleEntities_ShouldIncrementId_ShouldReturnEntityId()
        {
            // Arrange
            var entity = new MockEntity
            {
                Id = 1
            };

            // Act
            var actual1 = await _sut.SetAsync(entity);
            var actual2 = await _sut.SetAsync(entity);

            // Assert
            Assert.AreEqual(1, actual1);
            Assert.AreEqual(2, actual2);
        }

        [Test]
        public async Task ListAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var entity = new MockEntity
            {
                Id = 1
            };

            await _sut.SetAsync(entity);
            await _sut.SetAsync(entity);

            // Act
            var actual = await _sut.ListAllAsync();

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }
    }
}