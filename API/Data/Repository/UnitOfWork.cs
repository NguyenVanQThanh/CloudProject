using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;

namespace API.Data.Repository
{
    public class UnitOfWork(DataContext context, IUserRepository userRepository,
    IMessageRepository messageRepository, ILikesRepository likesRepository, IPhotoRepository photoRepository,
    ICategoryRepository categoryRepository, IProductRepository productRepository,
    IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, ICartRepository cartRepository,
    ICartItemRepository cartItemRepository) : IUnitOfWork
    {
        public IUserRepository UserRepository => userRepository;

        public IMessageRepository MessageRepository => messageRepository;

        public ILikesRepository LikesRepository => likesRepository;
        public IPhotoRepository PhotoRepository => photoRepository;

        public ICategoryRepository CategoryRepository => categoryRepository;

        public IProductRepository ProductRepository => productRepository;

        public ICartRepository CartRepository => cartRepository;

        public ICartItemRepository CartItemRepository => cartItemRepository;

        public IOrderRepository OrderRepository => orderRepository;

        public IOrderDetailRepository OrderDetailRepository => orderDetailRepository;

        public async Task<bool> Complete()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}