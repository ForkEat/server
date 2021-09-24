using System;
using FluentAssertions;
using ForkEat.Core.Domain;
using Xunit;

namespace ForkEat.Core.Tests.Domain
{
    public class StepTest
    {
        [Fact]
        public void Name_IsNotEmpty()
        {
            // Given
            var step = new Step(Guid.NewGuid(), "Test Name", "Test Instructions", new TimeSpan(0,1,0));
            
            // When & Then
            step.Invoking(s => s.Name = "")
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Step Name should not be null nor empty");
        }
        
        [Fact]
        public void Name_IsNotNull()
        {
            // Given
            var step = new Step(Guid.NewGuid(), "Test Name", "Test Instructions", new TimeSpan(0,1,0));
            
            // When & Then
            step.Invoking(s => s.Name = null)
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Step Name should not be null nor empty");
        }
        
        [Fact]
        public void Instructions_IsNotEmpty()
        {
            // Given
            var step = new Step(Guid.NewGuid(), "Test Name", "Test Instructions", new TimeSpan(0,1,0));
            
            // When & Then
            step.Invoking(s => s.Instructions = "")
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Step Instructions should not be null nor empty");
        }
        
        [Fact]
        public void Instructions_IsNotNull()
        {
            // Given
            var step = new Step(Guid.NewGuid(), "Test Name", "Test Instructions", new TimeSpan(0,1,0));
            
            // When & Then
            step.Invoking(s => s.Instructions = null)
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Step Instructions should not be null nor empty");
        }

        [Fact]
        public void EstimatedTime_NotZero()
        {
            // Given
            var step = new Step(Guid.NewGuid(), "Test Name", "Test Instructions", new TimeSpan(0,1,0));

            // When & Then
            step.Invoking(s => s.EstimatedTime = TimeSpan.Zero)
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Step EstimatedTime should not be 0");
        }
    }
}