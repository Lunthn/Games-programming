// array that contains the paths to the house models
const houses = [
  "House1.glb",
  "House2.glb",
  "Two story house1.glb",
  "Two story house2.glb",
  "Two story house3.glb",
  "Two story house4.glb",
  "Two story house5.glb",
  "Two story house6.glb",
];

export function buildHouse() {
  // randomly select a house from the array
  const randomHouseIndex = Math.floor(Math.random() * houses.length);
  const randomHouseString = houses[randomHouseIndex];

  const houseGroup = new THREE.Group();
  const gltfLoader = new THREE.GLTFLoader();

  // load the house
  gltfLoader.load("Suburban Houses Pack-glb/" + randomHouseString, (gltf) => {
    const houseModel = gltf.scene;

    houseModel.traverse((child) => {
      if (child.isMesh) {
        child.castShadow = true;
      }
    });

    houseGroup.add(houseModel);
  });

  return houseGroup;
}

export function buildFireHydrant() {
  const hydrantGroup = new THREE.Group();

  // create materials for the fire hydrant
  const redMaterial = new THREE.MeshStandardMaterial({
    color: 0xaa0000,
    roughness: 0.6,
  });
  const metalMaterial = new THREE.MeshStandardMaterial({
    color: 0x333333,
    metalness: 0.8,
    roughness: 0.2,
  });

  // create the fire hydrant body
  const bodyGeometry = new THREE.CylinderGeometry(0.5, 0.5, 1.2, 32);
  const body = new THREE.Mesh(bodyGeometry, redMaterial);
  body.castShadow = true;
  body.receiveShadow = true;
  hydrantGroup.add(body);

  // create various geometries that will be combined into a fire hydrant
  const topGeometry = new THREE.SphereGeometry(
    0.5,
    32,
    32,
    0,
    Math.PI * 2,
    0,
    Math.PI / 2,
  );
  const top = new THREE.Mesh(topGeometry, redMaterial);
  top.position.y = 0.6;
  top.castShadow = true;
  hydrantGroup.add(top);

  const nutGeometry = new THREE.CylinderGeometry(0.15, 0.15, 0.2, 6);
  const nut = new THREE.Mesh(nutGeometry, redMaterial);
  nut.position.y = 1.1;
  nut.castShadow = true;
  hydrantGroup.add(nut);

  const baseGeometry = new THREE.CylinderGeometry(0.65, 0.65, 0.2, 32);
  const base = new THREE.Mesh(baseGeometry, redMaterial);
  base.position.y = -0.6;
  base.castShadow = true;
  base.receiveShadow = true;
  hydrantGroup.add(base);

  const waterGeometry = new THREE.CylinderGeometry(0.2, 0.2, 0.4, 32);
  const water = new THREE.Mesh(waterGeometry, metalMaterial);
  water.rotation.x = Math.PI / 2;
  water.position.set(0, 0.2, 0.4);
  water.castShadow = true;
  hydrantGroup.add(water);

  hydrantGroup.scale.set(0.08, 0.08, 0.08);

  return hydrantGroup;
}

export function buildStreet(length) {
  // creating the texture of the streeet
  const loader = new THREE.TextureLoader();
  const streetTexture = loader.load("street-texture.jpg");
  const streetMat = new THREE.MeshStandardMaterial({ map: streetTexture });

  // setting the properties of the texture
  streetTexture.rotation = Math.PI / 2;
  streetTexture.wrapS = THREE.RepeatWrapping;
  streetTexture.wrapT = THREE.RepeatWrapping;
  streetTexture.repeat.set(length / 10, 1);

  // creating the street itself
  const streetGeom = new THREE.BoxGeometry(length, 1, 0.01);
  const street = new THREE.Mesh(streetGeom, streetMat);
  street.rotation.x = -Math.PI / 2;
  street.receiveShadow = true;

  return street;
}

export function buildTree() {
  const treeGroup = new THREE.Group();
  const gltfLoader = new THREE.GLTFLoader();

  // load the tree model
  gltfLoader.load("City Pack-glb/Tree.glb", (gltf) => {
    const tree = gltf.scene;

    tree.traverse((child) => {
      if (child.isMesh) {
        child.castShadow = true;
      }
    });

    treeGroup.add(tree);
  });

  // adjust scaling to compensate for model size
  treeGroup.scale.set(0.002, 0.002, 0.002);
  return treeGroup;
}

export function buildSun(x, y, z) {
  // create sun geometry
  const sunGeometry = new THREE.SphereGeometry(15, 32, 16);
  const sunMaterial = new THREE.MeshBasicMaterial({ color: "yellow" });
  const sun = new THREE.Mesh(sunGeometry, sunMaterial);

  sun.scale.set(0.1, 0.1, 0.1);

  // create a light source
  const light = new THREE.DirectionalLight(0xffffff, 1);
  light.castShadow = true;
  light.shadow.mapSize.width = 1024;
  light.shadow.mapSize.height = 1024;
  light.intensity = 0.7;

  light.position.set(x, y, z);
  sun.position.set(x, y, z);

  return { sun, light };
}

export function buildMonument() {
  const monumentGroup = new THREE.Group();

  const baseGeometry = new THREE.CylinderGeometry(1.15, 1, 5, 32);
  const base = new THREE.Mesh(
    baseGeometry,
    new THREE.MeshPhongMaterial({ color: "grey" }),
  );
  base.castShadow = true;
  base.receiveShadow = true;
  monumentGroup.add(base);

  const artGeometry = new THREE.TorusGeometry(10, 3, 100, 16);
  const art = new THREE.Mesh(artGeometry, new THREE.MeshNormalMaterial());
  art.receiveShadow = true;
  art.castShadow = true;
  art.scale.setScalar(0.08);
  art.position.y = 4;
  monumentGroup.add(art);

  const gltf = new THREE.GLTFLoader();
  gltf.load("teapot.glb", (gltf) => {
    const teapot = gltf.scene;

    teapot.scale.setScalar(0.3);

    teapot.position.y = 3.8;

    teapot.traverse((child) => {
      if (child.isMesh) {
        child.castShadow = true;
        child.receiveShadow = true;
        child.material = new THREE.MeshNormalMaterial();
      }
    });
    monumentGroup.add(teapot);
  });

  monumentGroup.userData.update = (time) => {
    art.rotation.x += 0.05;
    art.rotation.y -= 0.05;
    art.rotation.z -= 0.05;
  };

  monumentGroup.scale.setScalar(0.3);

  return monumentGroup;
}

export function buildGround() {
  // create top layer (grass)
  const grass = new THREE.Mesh(
    new THREE.BoxGeometry(20, 0.25, 20),
    new THREE.MeshStandardMaterial({ color: 0x6b8e23 }),
  );

  // create bottom layer (ground)
  const ground = new THREE.Mesh(
    new THREE.BoxGeometry(20, 0.5, 20),
    new THREE.MeshStandardMaterial({ color: 0x654321 }),
  );

  // set properties
  grass.position.y = -0.25;
  ground.position.y = -0.6125;
  grass.receiveShadow = true;

  return { grass, ground };
}
