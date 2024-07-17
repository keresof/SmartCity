using Xunit;
using UserManagement.Domain.ValueObjects;
using System;

namespace UserManagement.Domain.Tests;

public class PasswordTests
    {
        [Fact]
        public void Create_WithValidPassword_ShouldReturnPasswordObject()
        {
            // Arrange
            string plainTextPassword = "StrongPassword123!";

            // Act
            var password = Password.Create(plainTextPassword);

            // Assert
            Assert.NotNull(password);
            Assert.NotNull(password.Hash);
            Assert.NotEqual(plainTextPassword, password.Hash);
        }

        [Fact]
        public void Create_WithNullPassword_ShouldThrowArgumentException()
        {
            // Arrange
            string plainTextPassword = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Password.Create(plainTextPassword));
        }

        [Fact]
        public void Create_WithEmptyPassword_ShouldThrowArgumentException()
        {
            // Arrange
            string plainTextPassword = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Password.Create(plainTextPassword));
        }

        [Fact]
        public void Verify_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            string plainTextPassword = "StrongPassword123!";
            var password = Password.Create(plainTextPassword);

            // Act
            bool result = password.Verify(plainTextPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Verify_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            string correctPassword = "StrongPassword123!";
            string incorrectPassword = "WrongPassword456!";
            var password = Password.Create(correctPassword);

            // Act
            bool result = password.Verify(incorrectPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Create_WithTheSamePassword_ShouldReturnDifferentHashes()
        {
            // Arrange
            string plainTextPassword = "StrongPassword123!";

            // Act
            var password1 = Password.Create(plainTextPassword);
            var password2 = Password.Create(plainTextPassword);

            // Assert
            Assert.NotEqual(password1.Hash, password2.Hash);
            Assert.True(password1.Verify(plainTextPassword));
            Assert.True(password2.Verify(plainTextPassword));
        }
    }