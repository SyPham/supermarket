import { INavData } from '@coreui/angular'

// export const navItems: INavData[] = [
//   {
//     name: 'Dashboard',
//     url: '/dashboard',
//     icon: 'icon-speedometer',
//     badge: {
//       variant: 'info',
//       text: 'NEW'
//     }
//   },
//   {
//     title: true,
//     name: 'Theme'
//   },
//   {
//     name: 'Colors',
//     url: '/theme/colors',
//     icon: 'icon-drop'
//   },
//   {
//     name: 'Typography',
//     url: '/theme/typography',
//     icon: 'icon-pencil'
//   },
//   {
//     title: true,
//     name: 'Components'
//   },
//   {
//     name: 'Base',
//     url: '/base',
//     icon: 'icon-puzzle',
//     children: [
//       {
//         name: 'Cards',
//         url: '/base/cards',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Carousels',
//         url: '/base/carousels',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Collapses',
//         url: '/base/collapses',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Forms',
//         url: '/base/forms',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Navbars',
//         url: '/base/navbars',
//         icon: 'icon-puzzle'

//       },
//       {
//         name: 'Pagination',
//         url: '/base/paginations',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Popovers',
//         url: '/base/popovers',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Progress',
//         url: '/base/progress',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Switches',
//         url: '/base/switches',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Tables',
//         url: '/base/tables',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Tabs',
//         url: '/base/tabs',
//         icon: 'icon-puzzle'
//       },
//       {
//         name: 'Tooltips',
//         url: '/base/tooltips',
//         icon: 'icon-puzzle'
//       }
//     ]
//   },
//   {
//     name: 'Buttons',
//     url: '/buttons',
//     icon: 'icon-cursor',
//     children: [
//       {
//         name: 'Buttons',
//         url: '/buttons/buttons',
//         icon: 'icon-cursor'
//       },
//       {
//         name: 'Dropdowns',
//         url: '/buttons/dropdowns',
//         icon: 'icon-cursor'
//       },
//       {
//         name: 'Brand Buttons',
//         url: '/buttons/brand-buttons',
//         icon: 'icon-cursor'
//       }
//     ]
//   },
//   {
//     name: 'Charts',
//     url: '/charts',
//     icon: 'icon-pie-chart'
//   },
//   {
//     name: 'Icons',
//     url: '/icons',
//     icon: 'icon-star',
//     children: [
//       {
//         name: 'CoreUI Icons',
//         url: '/icons/coreui-icons',
//         icon: 'icon-star',
//         badge: {
//           variant: 'success',
//           text: 'NEW'
//         }
//       },
//       {
//         name: 'Flags',
//         url: '/icons/flags',
//         icon: 'icon-star'
//       },
//       {
//         name: 'Font Awesome',
//         url: '/icons/font-awesome',
//         icon: 'icon-star',
//         badge: {
//           variant: 'secondary',
//           text: '4.7'
//         }
//       },
//       {
//         name: 'Simple Line Icons',
//         url: '/icons/simple-line-icons',
//         icon: 'icon-star'
//       }
//     ]
//   },
//   {
//     name: 'Notifications',
//     url: '/notifications',
//     icon: 'icon-bell',
//     children: [
//       {
//         name: 'Alerts',
//         url: '/notifications/alerts',
//         icon: 'icon-bell'
//       },
//       {
//         name: 'Badges',
//         url: '/notifications/badges',
//         icon: 'icon-bell'
//       },
//       {
//         name: 'Modals',
//         url: '/notifications/modals',
//         icon: 'icon-bell'
//       }
//     ]
//   },
//   {
//     name: 'Widgets',
//     url: '/widgets',
//     icon: 'icon-calculator',
//     badge: {
//       variant: 'info',
//       text: 'NEW'
//     }
//   },
//   {
//     divider: true
//   },
//   {
//     title: true,
//     name: 'Extras',
//   },
//   {
//     name: 'Pages',
//     url: '/pages',
//     icon: 'icon-star',
//     children: [
//       {
//         name: 'Login',
//         url: '/login',
//         icon: 'icon-star'
//       },
//       {
//         name: 'Register',
//         url: '/register',
//         icon: 'icon-star'
//       },
//       {
//         name: 'Error 404',
//         url: '/404',
//         icon: 'icon-star'
//       },
//       {
//         name: 'Error 500',
//         url: '/500',
//         icon: 'icon-star'
//       }
//     ]
//   },
//   {
//     name: 'Disabled',
//     url: '/dashboard',
//     icon: 'icon-ban',
//     badge: {
//       variant: 'secondary',
//       text: 'NEW'
//     },
//     attributes: { disabled: true },
//   },
//   {
//     name: 'Download CoreUI',
//     url: 'http://coreui.io/angular/',
//     icon: 'icon-cloud-download',
//     class: 'mt-auto',
//     variant: 'success',
//     attributes: { target: '_blank', rel: 'noopener' }
//   },
//   {
//     name: 'Try CoreUI PRO',
//     url: 'http://coreui.io/pro/angular/',
//     icon: 'icon-layers',
//     variant: 'danger',
//     attributes: { target: '_blank', rel: 'noopener' }
//   }
// ];
export const navItems: INavData[] = [
  {
    name: 'Dashboard',
    url: '/dashboard',
    icon: 'icon-speedometer',
    badge: {
      variant: 'info',
      text: 'NEW'
    }
  },
  {
    name: '管理員',
    url: '/admin',
    icon: 'icon-puzzle',
    children: [
      {
        name: '帳號管理',
        url: '/admin/account',
        icon: 'icon-puzzle'
      },
      {
        name: '商店管理',
        url: '/admin/store',
        icon: 'icon-puzzle'
      },
      {
        name: '商品種類設定',
        url: '/admin/kind',
        icon: 'icon-puzzle'
      },
      {
        name: '商品維護',
        url: '/admin/product',
        icon: 'icon-puzzle'
      },
      {
        name: '統購清單',
        url: '/admin/order',
        icon: 'icon-puzzle'
      },
      // {
      //   name: 'Delivery',
      //   url: '/admin/delivery',
      //   icon: 'icon-puzzle'
      // }
    ]
  },
  {
    name: '使用者',
    url: '/consumer',
    icon: 'icon-bell',
    children: [
      {
        name: '更改密碼',
        url: '/consumer/change-password',
        icon: 'icon-bell'
      },
      {
        name: '商品資訊',
        url: '/consumer/product-list',
        icon: 'icon-bell'
      },
      {
        name: '訂單狀態',
        url: '/consumer/cart-status',
        icon: 'icon-bell'
      },
    ]
  },
];
export const navItemsVI: INavData[] = [
  {
    name: 'Dashboard',
    url: '/dashboard',
    icon: 'icon-speedometer',
    badge: {
      variant: 'info',
      text: 'NEW'
    }
  },
  {
    name: 'admin',
    url: '/admin',
    icon: 'icon-puzzle',
    children: [
      {
        name: 'Tài khoản',
        url: '/admin/account',
        icon: 'icon-puzzle'
      },
      {
        name: 'Cửa hàng',
        url: '/admin/store',
        icon: 'icon-puzzle'
      },
      {
        name: 'Loại',
        url: '/admin/kind',
        icon: 'icon-puzzle'
      },
      {
        name: 'Sản phẩm',
        url: '/admin/product',
        icon: 'icon-puzzle'
      },
      {
        name: 'Danh sách mua',
        url: '/admin/order',
        icon: 'icon-puzzle'
      },
      {
        name: 'Delivery',
        url: '/admin/delivery',
        icon: 'icon-puzzle'
      }
    ]
  },
  {
    name: 'Khách hàng',
    url: '/consumer',
    icon: 'icon-bell',
    children: [
      {
        name: 'Đổi mật khẩu',
        url: '/consumer/change-password',
        icon: 'icon-bell'
      },
      {
        name: 'Danh sách sản phẩm',
        url: '/consumer/product-list',
        icon: 'icon-bell'
      },
      {
        name: 'Tình trạng giỏ hàng',
        url: '/consumer/cart-status',
        icon: 'icon-bell'
      },
    ]
  },
];
export const navItemsEN: INavData[] = [
  {
    name: 'Dashboard',
    url: '/dashboard',
    icon: 'icon-speedometer',
    badge: {
      variant: 'info',
      text: 'NEW'
    }
  },
  {
    name: 'admin',
    url: '/admin',
    icon: 'icon-puzzle',
    children: [
      {
        name: 'Account',
        url: '/admin/account',
        icon: 'icon-puzzle'
      },
      {
        name: 'Store',
        url: '/admin/store',
        icon: 'icon-puzzle'
      },
      {
        name: 'Kind',
        url: '/admin/kind',
        icon: 'icon-puzzle'
      },
      {
        name: 'Product',
        url: '/admin/product',
        icon: 'icon-puzzle'
      },
      {
        name: 'Buy List',
        url: '/admin/order',
        icon: 'icon-puzzle'
      },
      {
        name: 'Delivery',
        url: '/admin/delivery',
        icon: 'icon-puzzle'
      }
    ]
  },
  {
    name: 'Consumer',
    url: '/consumer',
    icon: 'icon-bell',
    children: [
      {
        name: 'Change Password',
        url: '/consumer/change-password',
        icon: 'icon-bell'
      },
      {
        name: 'Product List',
        url: '/consumer/product-list',
        icon: 'icon-bell'
      },
      {
        name: 'Cart Status',
        url: '/consumer/cart-status',
        icon: 'icon-bell'
      },
    ]
  },
];
