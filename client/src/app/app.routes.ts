import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MembersListComponent } from './members/members-list/members-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { TestedErrorComponent } from './errors/tested-error/tested-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { memberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { adminGuard } from './_guards/admin.guard';
import { ProductDetailComponent } from './product/product-detail/product-detail.component';
import { productDetailedResolver } from './_resolvers/product-detailed.resolver';
import { ProductPanelComponent } from './product/product-panel/product-panel.component';
import { ProductOwnerComponent } from './product/product-owner/product-owner.component';
import { vendorGuard } from './_guards/vendor.guard';
import { productListedResolver } from './_resolvers/product-list.resolver';
import { CartPanelComponent } from './cart/cart-panel/cart-panel.component';
import { cartGuard } from './_guards/cart.guard';

export const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'products', component: ProductPanelComponent},
  {path: 'products/:id', component: ProductDetailComponent, resolve: {product: productDetailedResolver}},
  {path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      {path: 'members', component: MembersListComponent},
      {path: 'members/:username', component: MemberDetailComponent, resolve: {member: memberDetailedResolver}},
      {path: 'member/edit', component: MemberEditComponent, canDeactivate: [preventUnsavedChangesGuard]},
      {path: 'lists', component: ListsComponent},
      {path: 'messages', component: MessagesComponent},
      {path: 'cart', component: CartPanelComponent, canActivate: [cartGuard]},
      {path: 'owner', component: ProductOwnerComponent, canActivate:[vendorGuard]},
      {path: 'admin', component: AdminPanelComponent, canActivate: [adminGuard]}
    ]
  },
  {path: 'errors', component: TestedErrorComponent},
  {path: 'not-found', component: NotFoundComponent},
  {path: 'server-error', component: ServerErrorComponent},
  {path: '**', component: NotFoundComponent, pathMatch: 'full'},
];
